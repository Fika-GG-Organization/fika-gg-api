using System.ComponentModel.DataAnnotations;
using FikaGg.Api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FikaGg.Api.Features.User;

[ApiController]
[Authorize]
[Route("api/users/me/preferences")]
public class UserPreferencesController(
	UserManager<ApplicationUser> userManager) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get()
	{
		var user = await userManager.GetUserAsync(User);

		if (user is null)
		{
			return Unauthorized();
		}

		return Ok(new
		{
			themePreference = user.ThemePreference
		});
	}

	[HttpPut("theme")]
	public async Task<IActionResult> UpdateTheme(
		[FromBody] ThemePreferenceRequest request)
	{
		var user = await userManager.GetUserAsync(User);

		if (user is null)
		{
			return Unauthorized();
		}

		user.ThemePreference = request.ThemePreference;

		var result = await userManager.UpdateAsync(user);

		if (!result.Succeeded)
		{
			return Problem(
				title: "Could not save theme preference",
				statusCode: StatusCodes.Status500InternalServerError);
		}

		return NoContent();
	}
}
