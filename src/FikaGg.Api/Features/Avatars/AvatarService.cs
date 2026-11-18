using FikaGg.Api.Data;
using FikaGg.Api.Integrations.CloudinaryConfig;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.Avatars;

public class AvatarService(AppDbContext db, IImageStorageService storage)
{
	public async Task<string?> UploadAsync(string userId, IFormFile file, CancellationToken ct)
	{
		var user = await db.Users.FirstOrDefaultAsync(
			u => u.Id == userId, ct);

		if (user is null)
		{
			return null;
		}

		var oldPublicId = user.AvatarPublicId;

		await using var stream = file.OpenReadStream();
		var uploaded = await storage.UploadAvatarAsync(stream, file.FileName, userId, ct);

		user.AvatarUrl = uploaded.Url;
		user.AvatarPublicId = uploaded.PublicId;

		await db.SaveChangesAsync(ct);


		if (oldPublicId is not null)
		{
			await storage.DeleteAsync(oldPublicId, ct);
		}

		return uploaded.Url;
	}

	public async Task<bool> DeleteAsync(string userId, CancellationToken ct)
	{
		var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
		if (user is null)
		{
			return false;
		}

		var publicId = user.AvatarPublicId;

		if (publicId is null)
		{
			return true;
		}

		user.AvatarUrl = null;
		user.AvatarPublicId = null;

		await db.SaveChangesAsync(ct);
		await storage.DeleteAsync(publicId, ct);

		return true;
	}
}
