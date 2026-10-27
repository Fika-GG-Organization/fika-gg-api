using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.Events;

public class EventViewsService : IEventViewsService
{
	private readonly AppDbContext _context;
	public EventViewsService(AppDbContext context)
	{
		_context = context;
	}

	public Task<Event?> GetEventByIdAsync(int eventId)
	{
		throw new NotImplementedException();
	}
	public async Task<IEnumerable<GetEventDTO>> GetEventsAsync(string startDate, string endDate)
	{
		var events = await _context.Events
			.Where(e => e.StartTime >= Converter.DateConverter(startDate) && e.EndTime <= Converter.DateConverter(endDate))
			.Include(e => e.Game)
			.Select(e => new GetEventDTO
			{
				HostId = e.Host.Id,
				Title = e.Title,
				Description = e.Description,
				Game = e.Game.Name,
				StartTime = e.StartTime,
				EndTime = e.EndTime,
				CreatedAt = e.CreatedAt,
				Participants = e.Participants.Count()
			})
			.ToListAsync();

		return events;
	}

}
