using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Events;
using FikaGg.Api.Features.Game;
using FikaGg.Api.Features.User.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.User;

public class UserService : IUserService
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<ApplicationUser> _userManager;

	public UserService(AppDbContext dbContext, UserManager<ApplicationUser> userManager)
	{
		_dbContext = dbContext;
		_userManager = userManager;
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
		if(dto == null)
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

		if(!createResult.Succeeded)
		{
			return IdentityResult.Failed(createResult.Errors.ToArray());
		}

		return IdentityResult.Success;
	}

}
