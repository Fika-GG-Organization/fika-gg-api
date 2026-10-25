using FikaGg.Api.Data;
using Microsoft.EntityFrameworkCore;
using FikaGg.Api.Common.Utils;

namespace FikaGg.Api.Features.Events;

public class EventService(AppDbContext dbContext)
{
	public async Task<List<EventGetDTO>> GetEventCalendarAsync(string startDate, string endDate)
	{
		var events = await dbContext.Events
			.Where(e => e.StartTime >= Converter.DateConverter(startDate) && e.EndTime <= Converter.DateConverter(endDate))
			.Include(g => g.Game)
			.Select(e => new EventGetDTO(
				e.Id,
				e.Title,
				e.StartTime,
				e.EndTime,
				e.Description,
				e.GameId,
				e.Game.CoverUrl,
				e.Participants.Count(),
				e.MaxPlayers
			))
			.ToListAsync();
		return events;
	}

	public async Task<EventGetIdDTO> GetEventByIdAsync(int eventId)
	{
		var eventEntity = await dbContext.Events
			.Include(e => e.Game)
			.Include(e => e.Participants)
			.FirstOrDefaultAsync(e => e.Id == eventId);
		return eventEntity == null
			? throw new KeyNotFoundException($"Event with ID {eventId} not found.")
			: new EventGetIdDTO(
			eventEntity.Id,
			eventEntity.Title,
			eventEntity.StartTime,
			eventEntity.EndTime,
			eventEntity.Description,
			eventEntity.GameId,
			eventEntity.Game.CoverUrl,
			eventEntity.Participants.Count(),
			eventEntity.MaxPlayers,
			eventEntity.Participants.Select(p => new EventParticipantDTO(
				p.UserId,
				p.User.DisplayName,
				p.User.AvatarUrl,
				p.Status.ToString(),
				p.JoinedAt
			)).ToList()
		);
	}
}
