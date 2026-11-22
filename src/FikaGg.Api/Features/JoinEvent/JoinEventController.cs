using FikaGg.Api.Common.Utils;
using FikaGg.Api.Features.EventViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FikaGg.Api.Features.JoinEvent;

[ApiController]
[Route("api/event/")]
[Authorize]
public class JoinEventController(JoinEventService service) : ControllerBase
{
	[HttpGet]
	[HttpHead]
	[Route("checkOverlap")]
	[ProducesResponseType(typeof(OkResult), 200)]
	[ProducesResponseType(typeof(NoContentResult), 204)]
	public async Task<IActionResult> Check(
		[FromQuery] string startTime, [FromQuery] string endTime)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId == null)
		{
			return Unauthorized();
		}

		var result = await service.CheckIfEventsOverlapAsync(
			userId, Converter.DateConverter(startTime), Converter.DateConverter(endTime));

		return result ? Ok() : NoContent();
	}

	[HttpPost("{eventId:int}/join")]
	public async Task<ActionResult<GetEventByIdDTO>> Join([FromRoute] int eventId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId == null)
		{
			return Unauthorized();
		}

		try
		{
			var @event = await service.AddEventParticipantAsync(userId, eventId);
			return Ok(@event);
		}
		catch (ArgumentException ex)
		{
			return BadRequest(ex.Message);
		}
	}

	[HttpDelete("{eventId:int}/leave")]
	public async Task<ActionResult<GetEventByIdDTO>> Leave([FromRoute] int eventId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId == null)
		{
			return Unauthorized();
		}

		var @event = await service.RemoveEventParticipantAsync(userId, eventId);

		return Ok(@event);
	}
}
