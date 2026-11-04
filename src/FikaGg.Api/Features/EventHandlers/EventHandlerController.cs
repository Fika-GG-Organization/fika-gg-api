using FikaGg.Api.Features.EventHandlers;
using FikaGg.Api.Features.EventViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FikaGg.Api.Features.EventHandlers;

[Authorize]
[Route("api/events")]
[ApiController]
public class EventHandlerController(IEventHandlerService service) : ControllerBase
{
	private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

	[HttpPost("createEvent")]
	public async Task<ActionResult<GetEventByIdDTO>> Create([FromBody] CreateEventDTO createEventDto)
	{
		var newEvent = await service.CreateEventAsync(createEventDto, UserId);

		// CreatedAtAction returns the newly created event using our Get controller Action.
		return CreatedAtAction(
			actionName: nameof(EventViewsController.GetEventPage),
			controllerName: nameof(EventViewsController),
			routeValues: new { id = newEvent.Id },
			value: newEvent);
	}

	[HttpPut("updateEvent/{id:int}")]
	public async Task<ActionResult<GetEventByIdDTO>> Update(
		[FromRoute] int id, [FromBody] CreateEventDTO eventDto)
	{
		var @event = await service.UpdateEventAsync(id, eventDto, UserId);

		return Ok(@event);
	}

	[HttpDelete("deleteEvent/{id:int}")]
	public async Task<IActionResult> Delete([FromRoute] int id)
	{
		await service.DeleteEventAsync(id, UserId);

		return NoContent();
	}

}
