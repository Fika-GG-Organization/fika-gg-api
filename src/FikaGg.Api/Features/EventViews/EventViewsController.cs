using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FikaGg.Api.Features.EventViews;

[Route("api/[controller]")]
[ApiController]
public class EventViewsController(IEventViewsService service) : ControllerBase
{

	[HttpGet("/getAll")]
	public async Task<IActionResult> GetAll(string startTime, string endTime)
	{
		var events = await service.GetEventsAsync(startTime, endTime);
		return Ok(events);
	}

}
