using FikaGg.Api.Data;
using FikaGg.Api.Entities;
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
			   .Select(e => e.Event)
			   .ToListAsync();

		var UserHostedEvents = await _dbContext.Events
				.AsNoTracking()
				.Where(e => e.HostId == userId)
				.ToListAsync();

		var UserGames = await _dbContext.UserGames
				.AsNoTracking()
				.Where(g => g.UserId == userId)
				.Select(g => g.Game)
				.ToListAsync();

		return new UserDataDTO
		{
			Id = user.Id,
			DisplayName = user.DisplayName,
			AvatarUrl = user.AvatarUrl,
			CreatedAt = user.CreatedAt,
			UserEvents = UserAttendingEvents,
			HostedEvents = UserHostedEvents,
			OwnedGames = UserGames

		};
	}


}
