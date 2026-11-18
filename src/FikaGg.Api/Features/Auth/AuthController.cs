using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FikaGg.Api.Features.Auth;

[ApiController]
[Route("api/auth")]
[Authorize]
public class AuthController(AuthService authService) : ControllerBase
{
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		await authService.LogoutAsync();
		return NoContent();
	}
}
