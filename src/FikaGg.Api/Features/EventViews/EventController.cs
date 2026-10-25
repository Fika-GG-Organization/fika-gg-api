using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.Events;

[ApiController]
[Route("api/events")]
public class EventController(EventService eventService) : ControllerBase
{
	[HttpGet("calendar")]
	public async Task<ActionResult<IEnumerable<EventGetDTO>>> GetEventCalendar(
		[FromQuery(Name = "start-date"), Required]string startDate, [FromQuery(Name = "end-date"), Required]string endDate)
	{
		var events = await eventService.GetEventCalendarAsync(startDate, endDate);
		return Ok(events);
	}

	[HttpGet("{eventId:int}")]
	public async Task<ActionResult<EventGetIdDTO>> GetEventById([FromRoute] int eventId)
	{
		var eventDetails = await eventService.GetEventByIdAsync(eventId);
		return Ok(eventDetails);
	}

	// Future search controller

	//[HttpGet("search")]
	//public async Task<ActionResult<IEnumerable<EventGetDTO>>> GetSearchEventCalendar(
	//	[FromQuery(Name = "start-date"), Required] string startDate, [FromQuery(Name = "end-date"), Required] string endDate)
	//{
	//	var events = await eventService.GetEventCalendarAsync(startDate, endDate);
	//	return Ok(events);
	//}
}
