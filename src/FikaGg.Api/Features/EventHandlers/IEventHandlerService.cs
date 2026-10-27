using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;

namespace FikaGg.Api.Features.EventHandlers;

public interface IEventHandlerService
{
	Task<Event> CreateEventAsync(CreateEventDTO createEventDto, string userId);
	Task<Event> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto, string userId);
	Task<Event> DeleteEventAsync(int eventId, string userId);

}
