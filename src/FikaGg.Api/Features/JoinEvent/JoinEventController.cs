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
	[HttpPost("{eventId:int}/join")]
	public async Task<ActionResult<GetEventByIdDTO>> Join(
		[FromRoute] int eventId,
		[FromBody] string joinedAt
		)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId == null)
		{
			return Unauthorized();
		}

		var @event = await service.AddEventParticipantAsync(userId, eventId, joinedAt);

		return Ok(@event);
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
