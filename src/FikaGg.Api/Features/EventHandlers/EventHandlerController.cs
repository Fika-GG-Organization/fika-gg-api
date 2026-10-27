using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FikaGg.Api.Features.Events;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EventHandlerController(IEventHandlerService service) : ControllerBase
{

	[HttpPost("/createEvent")]
	public async Task<IActionResult> Create([FromBody] CreateEventDTO createEventDto)
	{
		var newEvent = await service.CreateEventAsync(createEventDto);
		return Ok(newEvent);
	}

}
