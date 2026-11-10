namespace FikaGg.Api.Features.User;

using FikaGg.Api.Entities;
using global::FikaGg.Api.Features.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController(UserService userService) : ControllerBase
{
	[HttpGet("{id}")]
	public async Task<ActionResult<List<UserDataDTO>>> Search(
		[FromRoute, Required, MaxLength(100)] string id)
	{
		var results = await userService.GetUserAsync(id);
		return Ok(results);
	}

	[HttpGet("current")]
	[Authorize]
	public async Task<ActionResult<UserDataDTO>> GetCurrentUserData()
	{
		
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
		{
			return Unauthorized();
		}

		var userData = await userService.GetUserAsync(userId);
		return Ok(userData);
	}

	[AllowAnonymous]
	[HttpPost("register")]
	public async Task<ActionResult<UserDataDTO>> RegisterUser([FromBody] RegisterUserDTO dto)
	{
		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		var result = await userService.RegisterUserAsync(dto);
		if (!result.Succeeded)
		{
			return BadRequest(result.Errors);
		}

		return Ok();
	}
}
