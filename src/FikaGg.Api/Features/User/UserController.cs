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
		if (!ModelState.IsValid)
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

	[HttpPost("add/game/{gameId}")]
	public async Task<ActionResult> AddGameToUser([FromRoute] int gameId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			return Unauthorized();
		}

		try
		{
			await userService.AddGameToUserAsync(userId, gameId);
			return Ok();
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(ex.Message);
		}
	}

	// Endpoint is strictly for authentication purposes
	[HttpGet("me")]
	public async Task<ActionResult<AuthUserDTO>> GetMe()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrEmpty(userId))
		{
			return Unauthorized();
		}
		var user = await userService.GetAuthUserAsync(userId);

		return user is null ? Unauthorized() : Ok(user);
	}

	// Endpoint will run delete user that deletes the account and all associated data.
	[HttpDelete("delete")]
	public async Task<ActionResult> DeleteUserAccount()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			return Unauthorized();
		}
		try
		{
			await userService.DeleteUser(userId);
			return Ok();
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(ex.Message);
		}

	}
	[HttpPut("update/game/{gameId:int}")]
	public async Task<ActionResult> UpdateGamerTag([FromRoute] int gameId, [FromBody] UpdateGamerTagDTO dto)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			return Unauthorized();
		}

		await userService.UpdateGamerTagAsync(userId, gameId, dto.GamerTag);
		return NoContent();
	}

	[HttpDelete("remove/game/{gameId:int}")]
	public async Task<ActionResult> RemoveGameFromUser([FromRoute] int gameId)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrEmpty(userId))
		{
			return Unauthorized();
		}

		await userService.RemoveGameFromUserAsync(userId, gameId);
		return NoContent();
	}

	[HttpPut("UpdateProfile")]
	public async Task<ActionResult> UpdateUserProfile(UpdateProfileDTO dto)
	{
		try
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await userService.UpdateUserProfileAsync(dto, userId!);
			if (result.Succeeded)
			{
				return Ok();
			}
			else
			{
				return BadRequest();
			}
		}
		catch (InvalidOperationException ex)
		{
			return BadRequest(ex.Message);
		}
	}
}
