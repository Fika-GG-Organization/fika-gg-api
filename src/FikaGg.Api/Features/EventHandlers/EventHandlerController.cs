using FikaGg.Api.Features.EventHandlers;
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
	public async Task<IActionResult> Create([FromBody] CreateEventDTO createEventDto)
	{
		var newEvent = await service.CreateEventAsync(createEventDto, UserId);

		return CreatedAtAction(
			actionName: nameof(EventViews.EventViewsController.GetEventPage),
			controllerName: nameof(EventViews.EventViewsController),
			routeValues: new { id = newEvent.Id },
			value: newEvent);
	}

}
