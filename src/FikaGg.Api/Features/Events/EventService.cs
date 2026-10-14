using FikaGg.Api.Data;
using FikaGg.Api.Entities;

namespace FikaGg.Api.Features.Events;

public class EventService : IEventService
{
	private readonly AppDbContext _context;
	public EventService(AppDbContext context)
	{
		_context = context;
	}
	async public Task<Event> CreateEventAsync(CreateEventDTO createEventDto)
	{
		var newEvent = new Event
		{
			HostId = createEventDto.HostId,
			GameId = createEventDto.GameId,
			Title = createEventDto.Title,
			Description = createEventDto.Description,
			StartTime = createEventDto.StartTime,
			EndTime = createEventDto.EndTime,
			MaxPlayers = createEventDto.MaxPlayers,
			CreatedAt = DateTime.UtcNow
		};
		_context.Events.Add(newEvent);
		await _context.SaveChangesAsync();
		return newEvent;
	}
}
