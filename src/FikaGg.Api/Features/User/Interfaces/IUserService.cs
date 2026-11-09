using Microsoft.AspNetCore.Identity;

namespace FikaGg.Api.Features.User.Interfaces;

public interface IUserService
{
	Task<UserDataDTO> GetUserAsync(string userId);
	Task<IdentityResult> RegisterUserAsync(RegisterUserDTO dto);
}
