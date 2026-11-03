using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using FikaGg.Api.Features.EventViews;

namespace FikaGg.Api.Features.EventHandlers;

public interface IEventHandlerService
{
	Task<GetEventByIdDTO> CreateEventAsync(CreateEventDTO createEventDto, string userId);
	Task<Event> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto, string userId);
	Task<Event> DeleteEventAsync(int eventId, string userId);

}
