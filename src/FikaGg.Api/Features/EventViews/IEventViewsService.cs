using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;

namespace FikaGg.Api.Features.EventViews;

public interface IEventViewsService
{
	Task<Event?> GetEventByIdAsync(int eventId);
	Task<IEnumerable<GetEventDTO>> GetEventsAsync(string startDate, string endDate);
}
