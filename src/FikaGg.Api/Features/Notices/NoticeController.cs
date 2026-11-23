using FikaGg.Api.Features.Notices.DTOs;
using FikaGg.Api.Features.Notices.Interfaces;
using FikaGg.Api.Features.Notices.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FikaGg.Api.Features.Notices;

[Route("api/Notices")]
[ApiController]
[Authorize]
public class NoticeController(INoticeService service) : ControllerBase
{
	private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
		?? throw new UnauthorizedAccessException();

	[HttpGet]
	public async Task<ActionResult<List<NoticeDTO>>> GetNotices([FromQuery] bool unreadOnly = false)
	{
		try
		{
			var result = await service.GetNoticesAsync(UserId, unreadOnly);

			return Ok(result);
		}
		catch
		{
			return BadRequest();
		}
	}

	[HttpGet("unread-count")]
	public async Task<ActionResult<int>> GetUnreadCount()
	{
		try
		{
			var result = await service.GetUnreadCountAsync(UserId);
			return Ok(result);
		}
		catch
		{
			return BadRequest();
		}
	}

	[HttpPatch("{id:int}/viewed")]
	public async Task<IActionResult> MarkAsViewed(int id)
	{
		try
		{
			await service.MarkAsViewedAsync(id, UserId);
			return NoContent();
		}
		catch(KeyNotFoundException)
		{
			return NotFound();
		}
	}

	[HttpPatch("viewed")]
	public async Task<ActionResult<int>> MarkAllAsViewed()
	{
		try
		{
			var result = await service.MarkAllAsViewedAsync(UserId);
			return Ok(result);
		}
		catch
		{
			return BadRequest();
		}
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> Delete(int Id)
	{
		try
		{
			await service.DeleteNoticeAsync(Id, UserId);
			return NoContent();
		}
		catch(KeyNotFoundException)
		{
			return NotFound();
		}
	}
}
