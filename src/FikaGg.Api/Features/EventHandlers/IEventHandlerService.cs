using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;

namespace FikaGg.Api.Features.Events;

public interface IEventHandlerService
{
	Task<Event> CreateEventAsync(CreateEventDTO createEventDto);
	Task<Event> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto);
	Task<Event> DeleteEventAsync(int eventId);

}
