using FikaGg.Api.Features.EventHandlers;
using FikaGg.Api.Features.EventViews;

namespace FikaGg.Api.Features.EventHandlers;

public interface IEventHandlerService
{
	Task<GetEventByIdDTO> CreateEventAsync(CreateEventDTO createEventDto, string userId);
	Task<GetEventByIdDTO> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto, string userId);
	Task DeleteEventAsync(int eventId, string userId);

}
