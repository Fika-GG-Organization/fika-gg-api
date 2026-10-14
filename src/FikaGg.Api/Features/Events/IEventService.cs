using FikaGg.Api.Entities;

namespace FikaGg.Api.Features.Events;

public interface IEventService
{
	Task<Event> CreateEventAsync(CreateEventDTO createEventDto);

}
