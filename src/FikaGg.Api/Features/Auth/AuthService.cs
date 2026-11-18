using FikaGg.Api.Entities;
using Microsoft.AspNetCore.Identity;

namespace FikaGg.Api.Features.Auth;

public class AuthService(SignInManager<ApplicationUser> signInManager)
{
	public Task LogoutAsync() => signInManager.SignOutAsync();
}
