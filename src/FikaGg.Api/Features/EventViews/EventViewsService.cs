using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using FikaGg.Api.Features.EventViews;

namespace FikaGg.Api.Features.EventViews;

public class EventViewsService(AppDbContext context) : IEventViewsService
{
	public Task<Event?> GetEventByIdAsync(int eventId)
	{
		throw new NotImplementedException();
	}
	public async Task<IEnumerable<GetEventDTO>> GetEventsAsync(string startDate, string endDate)
	{
		var events = await context.Events
			.AsNoTracking()
			.Where(e => e.StartTime >= Converter.DateConverter(startDate) && e.EndTime <= Converter.DateConverter(endDate))
			.Select(e => new GetEventDTO(

				e.Id,
				e.Host.Id,
				e.Title,
				e.Description,
				e.Game.Id,
				e.StartTime,
				e.EndTime,
				e.Game.CoverUrl,
				e.Participants.Count(),
				e.MaxPlayers,
				e.CreatedAt
				)
			)
			.ToListAsync();

		return events;
	}

}
