namespace FikaGg.Api.Features.User;

using global::FikaGg.Api.Features.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;



[ApiController]
[Route("api/user/{id}")]
[Authorize]
public class UserController(UserService userService) : ControllerBase
{
	[HttpGet("search")]
	public async Task<ActionResult<List<UserDataDTO>>> Search(
		[FromQuery, Required, MaxLength(100)] string query)
	{
		var results = await userService.GetUserAsync(query);
		return Ok(results);
	}
}
