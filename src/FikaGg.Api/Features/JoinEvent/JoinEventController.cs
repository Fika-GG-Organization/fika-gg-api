using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FikaGg.Api.Features.JoinEvent;

[ApiController]
[Route("api/events/")]
[Authorize]
public class JoinEventController(JoinEventService service) : ControllerBase
{
	[HttpPost("join")]
	public async Task<ActionResult<int>> Join([FromBody] DTOJoinEvent dto)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId == null)
		{
			return Unauthorized();
		}

		var eventId = await service.AddUserParticipant(userId, dto);

		return Ok(eventId);
	}
}
