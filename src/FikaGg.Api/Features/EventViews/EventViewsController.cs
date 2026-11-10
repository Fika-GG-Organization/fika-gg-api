using FikaGg.Api.Features.EventHandlers;
using Microsoft.AspNetCore.Mvc;

namespace FikaGg.Api.Features.EventViews;

[Route("api/events")]
[ApiController]
public class EventViewsController(IEventViewsService service) : ControllerBase
{
	[HttpGet("getById/{id:int}")]
	public async Task<ActionResult<GetEventByIdDTO>> GetEventPage([FromRoute] int id)
	{
		var result = await service.GetEventByIdAsync(id);

		return Ok(result);
	}


	[HttpGet("getCalendar")]
	public async Task<ActionResult<IEnumerable<GetEventDTO>>> GetAll(string startTime, string endTime)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(startTime);
		ArgumentException.ThrowIfNullOrWhiteSpace(endTime);
		var events = await service.GetEventsAsync(startTime, endTime);
		return Ok(events);
	}

}
