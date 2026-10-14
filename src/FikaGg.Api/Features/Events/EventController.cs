using FikaGg.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FikaGg.Api.Features.Events;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
	private readonly IEventService _eventService;
	public EventController(IEventService service)
	{
		_eventService = service;
	}

	[HttpPost]
	public async Task<IActionResult> Post([FromBody] CreateEventDTO createEventDto)
	{
		var newEvent = await _eventService.CreateEventAsync(createEventDto);
		return Ok(newEvent);
	}

}
