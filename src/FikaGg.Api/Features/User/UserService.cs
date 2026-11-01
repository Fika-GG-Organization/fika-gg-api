using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Events;
using FikaGg.Api.Features.Game;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.User;

public class UserService
{
	private readonly AppDbContext _dbContext;

	public UserService(AppDbContext dbContext)
	{
		_dbContext = dbContext;
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


}
