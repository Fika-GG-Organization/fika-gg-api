using FikaGg.Api.Integrations.CloudinaryConfig;
using Microsoft.AspNetCore.Mvc;

namespace FikaGg.Api.Features.Cloudinary;

[ApiController]
[Route("api/profile-pictures")]
public class ProfilePictureController(IImageStorageService storage) : ControllerBase
{
	[HttpPost]
	[Consumes("multipart/form-data")]
	public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
	{
		await using var stream = file.OpenReadStream();
		var result = await storage.UploadProfilePictureAsync(stream, file.FileName, ct);
		return Ok(result);
	}
}
