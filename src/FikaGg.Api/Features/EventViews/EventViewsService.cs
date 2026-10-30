using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Features.EventViews;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.EventViews;

public class EventViewsService(AppDbContext context) : IEventViewsService
{
	public async Task<GetEventByIdDTO?> GetEventByIdAsync(int eventId)
	{
		var result = await context.Events.Where(e => e.Id == eventId)
			.Include(e => e.Game)
			.Include(e => e.Host)
			.Include(e => e.Participants)
			.Select(e => new GetEventByIdDTO(
				e.Id,
				e.HostId,
				e.Title,
				e.Description,
				e.GameId,
				e.StartTime,
				e.EndTime,
				(e.Game == null) ? null : e.Game.CoverUrl, // If Game is null, CoverUrl will also be null
				e.Participants.Count,
				e.MaxPlayers,
				e.Participants.Select(
					p => new GetEventParticipantDTO(p.EventId, p.UserId, p.Status, p.JoinedAt)).ToList(),
				e.CreatedAt)
			)
			.FirstOrDefaultAsync();

		return result ?? null;
	}

	public async Task<IEnumerable<GetEventDTO>> GetEventsAsync(string startDate, string endDate)
	{
		var events = await context.Events
			.AsNoTracking()
			.Where(e => e.StartTime >= Converter.DateConverter(startDate) && e.StartTime <= Converter.DateConverter(endDate))
			.Select(e => new GetEventDTO(

				e.Id,
				e.Host!.Id,
				e.Title,
				e.Description,
				(e.Game == null) ? null : e.Game.Id,
				e.StartTime,
				e.EndTime,
				(e.Game == null) ? null : e.Game.CoverUrl,
				e.Participants.Count(),
				e.MaxPlayers,
				e.CreatedAt
				)
			)
			.ToListAsync();

		return events;
	}

}
