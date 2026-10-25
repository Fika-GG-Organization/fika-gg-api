using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FikaGg.Api.Features.EventHandlers;

[Authorize]
[ApiController]
[Route("api/events")]
public class EventController(EventService eventService) : ControllerBase
{
	private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

	[HttpPost]
	public async Task<IActionResult> CreateEvent([FromBody] EventCreateDTO dto)
	{
		var response = await eventService.CreateAsync(dto, UserId);
	}
}
