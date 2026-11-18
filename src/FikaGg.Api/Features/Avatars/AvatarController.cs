using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FikaGg.Api.Features.Avatars;

[ApiController]
[Route("api/users/me/avatar")]
[Authorize]
public class AvatarController(AvatarService service) : ControllerBase
{
	private const long MaxFileBytes = 5 * 1024 * 1024; // 5MB
	private const long MaxRequestBytes = MaxFileBytes + 1024 * 1024; // 6MB
	private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png", "image/webp"];

	[HttpPut]
	[Consumes("multipart/form-data")]
	[RequestSizeLimit(MaxRequestBytes)]
	public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId is null)
		{
			return Unauthorized();
		}

		if (file.Length == 0)
		{
			return Problem(statusCode: 400, detail: "The file is empty.");
		}

		if (file.Length > MaxFileBytes)
		{
			return Problem(statusCode: 400, detail: "The file can be at most 5 MB.");
		}

		if (!AllowedContentTypes.Contains(file.ContentType))
		{
			return Problem(statusCode: 400, detail: "Only filetype of JPEG, PNG and WebP are allowed.");
		}

		var avatarUrl = await service.UploadAsync(userId, file, ct);

		return avatarUrl is null ? NotFound() : Ok(new AvatarResponse(avatarUrl));
	}

	[HttpDelete]
	public async Task<IActionResult> Delete(CancellationToken ct)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (userId is null)
		{
			return Unauthorized();
		}

		var found = await service.DeleteAsync(userId, ct);

		return found ? NoContent() : NotFound();
	}
}
