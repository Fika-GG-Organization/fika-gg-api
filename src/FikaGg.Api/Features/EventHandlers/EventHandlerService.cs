using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.Events;

public class EventHandlerService : IEventHandlerService
{
	private readonly AppDbContext _context;
	public EventHandlerService(AppDbContext context)
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
		_context.Events.AddAsync(newEvent);
		await _context.SaveChangesAsync();
		return newEvent;
	}
	public Task<Event> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto) => throw new NotImplementedException();
	public Task<Event> DeleteEventAsync(int eventId) => throw new NotImplementedException();
}
