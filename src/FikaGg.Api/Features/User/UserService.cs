using CloudinaryDotNet.Actions;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Events;
using FikaGg.Api.Features.Game;
using FikaGg.Api.Features.User.Interfaces;
using FikaGg.Api.Integrations.Rawg;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;


namespace FikaGg.Api.Features.User;

public class UserService : IUserService
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IRawgClient _rawgClient;

	public UserService(AppDbContext dbContext, UserManager<ApplicationUser> userManager, IRawgClient rawgClient)
	{
		_dbContext = dbContext;
		_userManager = userManager;
		_rawgClient = rawgClient;
	}

	public async Task<UserDataDTO> GetUserAsync(string userId)
	{
		var user = await _dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == userId);

		if (user == null)
			throw new InvalidOperationException("User not found");
		var UserAttendingEvents = await _dbContext.EventParticipants
			   .AsNoTracking()
			   .Where(e => e.UserId == userId)
			   .Select(e => new EventGetDTO(
				   e.EventId,
				   e.Event!.Title,
				   e.Event.StartTime,
				   e.Event.EndTime,
				   e.Event.Description,
				   e.Event.GameId,
				   e.Event.Participants.Count,
				   e.Event.MaxPlayers
			   ))
			   .ToListAsync();

		var UserHostedEvents = await _dbContext.Events
				.AsNoTracking()
				.Where(e => e.HostId == userId)
				.Select(e => new EventGetDTO(
					e.Id,
					e.Title,
					e.StartTime,
					e.EndTime,
					e.Description,
					e.GameId,
					e.Participants.Count,
					e.MaxPlayers
				))
				.ToListAsync();

		var UserGames = await _dbContext.UserGames
				.AsNoTracking()
				.Where(g => g.UserId == userId)
				.Select(g => new UserGameDTO(
					g.GameId,
					g.Game!.RawgId,
					g.Game.Name,
					g.Game!.CoverUrl,
					g.GamerTag
				))
				.ToListAsync();

		return new UserDataDTO(
			user.Id,
			user.DisplayName,
			user.AvatarUrl,
			UserAttendingEvents,
			UserHostedEvents,
			UserGames
		);
	}


	/// <summary>
	/// Custom Identity login method that creates a new user with Displayname requirements and an optional avatar URL parameter on top of the default Identity behavior.
	/// </summary>
	/// <param name="dto"></param>
	/// <returns></returns>
	public async Task<IdentityResult> RegisterUserAsync(RegisterUserDTO dto)
	{
		if (dto == null)
		{
			return IdentityResult.Failed(new IdentityError
			{
				Description = "RegisterUserDTO cannot be null"
			});
		}
		var user = new ApplicationUser
		{
			DisplayName = dto.DisplayName,
			Email = dto.Email,
			UserName = dto.Email,
			AvatarUrl = null // Set a default avatar URL if needed
		};

		var displayNameTaken = await _dbContext.Users
		.AnyAsync(u => u.DisplayName == dto.DisplayName);

		if (displayNameTaken)
		{
			return IdentityResult.Failed(new IdentityError
			{
				Code = "DuplicateDisplayName",
				Description = "A user with this display name already exists"
			});
		}
		var createResult = await _userManager.CreateAsync(user, dto.Password);

		if (!createResult.Succeeded)
		{
			return IdentityResult.Failed(createResult.Errors.ToArray());
		}

		return IdentityResult.Success;
	}

	/// <summary>
	/// Profile update method that checks for changes in Email, passwords and display names,
	/// checks validity of incoming changes and applies valid changes to the profile.
	/// Will roll back changes if it fails, maintaining original data.
	/// </summary>
	/// <param name="dto"></param>
	/// <param name="userId"></param>
	/// <returns></returns>
	public async Task<IdentityResult> UpdateUserProfileAsync(UpdateProfileDTO dto, string userId)
	{
		if (dto == null)
			return IdentityResult.Failed(new IdentityError
			{
				Code = "EmptyObject",
				Description = "Incoming object cannot be null, no changes applied"
			});

		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
			return IdentityResult.Failed(new IdentityError
			{
				Code = "NoUser",
				Description = "User not Found"
			});

		var newName = dto.DisplayName?.Trim();
		var changingName = !string.IsNullOrWhiteSpace(newName) && newName != user.DisplayName;
		var changingEmail = !string.IsNullOrWhiteSpace(dto.Email)
			&& !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase);
		var changingPassword = !string.IsNullOrEmpty(dto.NewPassword);

		if (changingEmail || changingPassword)
		{
			if (string.IsNullOrEmpty(dto.CurrentPassword) || !await _userManager.CheckPasswordAsync(user, dto.CurrentPassword))
			{
				return IdentityResult.Failed(new IdentityError
				{
					Code = "CredentialMismatch",
					Description = "Credentials do not match, no changes applied"
				});
			}
		}

		var strategy = _dbContext.Database.CreateExecutionStrategy();
		return await strategy.ExecuteAsync(async () =>
		{
			await using var transaction = await _dbContext.Database.BeginTransactionAsync();

			if (changingName)
			{
				var nameTaken = await _dbContext.Users
							.AnyAsync(u => u.DisplayName == newName &&
						u.Id != user.Id);
				if (nameTaken)
				{
					return IdentityResult.Failed(new IdentityError
					{
						Code = "DuplicateDisplayName",
						Description = "Displayname is already taken."
					});
				}
				user.DisplayName = newName!;
				var result = await _userManager.UpdateAsync(user);
				if (!result.Succeeded)
					return result;
			}

			if (changingEmail)
			{
				var token = await _userManager.GenerateChangeEmailTokenAsync(user, dto.Email!);
				var result = await _userManager.ChangeEmailAsync(user, dto.Email!, token);
				if (!result.Succeeded)
					return result;

				result = await _userManager.SetUserNameAsync(user, dto.Email!);
				if (!result.Succeeded)
					return result;
			}

			if (changingPassword)
			{
				var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword!, dto.NewPassword!);
				if (!result.Succeeded)
					return result;
			}

			await transaction.CommitAsync();
			return IdentityResult.Success;
		});
	}

	

	public async Task AddGameToUserAsync(string userId, int gameRawgId)
	{
		var user = await _dbContext.Users.FindAsync(userId);
		if (user == null)
		{
			throw new InvalidOperationException("User not found");
		}

		var game = await _dbContext.Games.FirstOrDefaultAsync(g => g.RawgId == gameRawgId);

		if (game == null)
		{
			var rawgGameDetails = await _rawgClient.GetGameDetailsAsync(gameRawgId);
			if (rawgGameDetails == null)
			{
				throw new InvalidOperationException("Game not found on RAWG");
			}

			game = new Entities.Game
			{
				RawgId = rawgGameDetails.Id,
				Name = rawgGameDetails.Name,
				CoverUrl = rawgGameDetails.BackgroundImage
			};

			_dbContext.Games.Add(game);
			await _dbContext.SaveChangesAsync();
		}

		var alreadyOwned = await _dbContext.UserGames
			.AnyAsync(ug => ug.UserId == userId && ug.GameId == game.Id);

		if (alreadyOwned)
		{
			throw new InvalidOperationException("User already owns this game");
		}

		var userGame = new UserGame
		{
			UserId = userId,
			GameId = game.Id
		};

		
		

		await _dbContext.UserGames.AddAsync(userGame);
		await _dbContext.SaveChangesAsync();
	}
	public async Task<AuthUserDTO?> GetAuthUserAsync(string userId)
	{
		return await _dbContext.Users
			.AsNoTracking()
			.Where(u => u.Id == userId)
			.Select(u => new AuthUserDTO(
				u.Id,
				u.Email!,
				u.DisplayName,
				u.AvatarUrl
			))
			.FirstOrDefaultAsync();
	}

	public async Task<IdentityResult> DeleteUser(string userId)
	{
		var found = await _userManager.FindByIdAsync(userId);

		if (found == null)
		{
			return IdentityResult.Failed(new IdentityError
			{
				Code = "NoUser",
				Description = "User not found"
			});
		}

		return await _userManager.DeleteAsync(found);
	}
	public async Task UpdateGamerTagAsync(string userId, int gameId, string? gamerTag)
	{
		var userGame = await _dbContext.UserGames
			.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);

		if (userGame == null)
		{
			throw new KeyNotFoundException("User does not have this game");
		}

		userGame.GamerTag = string.IsNullOrWhiteSpace(gamerTag) ? null : gamerTag.Trim();
		await _dbContext.SaveChangesAsync();
	}

	public async Task RemoveGameFromUserAsync(string userId, int gameId)
	{
		var userGame = await _dbContext.UserGames
			.FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GameId == gameId);

		if (userGame == null)
		{
			throw new KeyNotFoundException("User does not have this game");
		}

		// removes it from the user, but keeps it as a game entity.
		_dbContext.UserGames.Remove(userGame);
		await _dbContext.SaveChangesAsync();
	}
}
