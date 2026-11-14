using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Events;
using FikaGg.Api.Features.Game;
using FikaGg.Api.Features.User.Interfaces;
using FikaGg.Api.Integrations.Rawg;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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
				   e.Event.Id,
				   e.Event.Title,
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
				.Select(g => new GameSearchResultDTO(
					g.Game.RawgId ?? 0,
					g.Game.Name,
					g.Game.CoverUrl
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
}
