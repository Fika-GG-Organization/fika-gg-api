namespace FikaGg.Api.Integrations.CloudinaryConfig;

public interface IImageStorageService
{
	Task<UploadedImage> UploadAvatarAsync(Stream stream, string fileName, string userId, CancellationToken ct);
	Task DeleteAsync(string publicId, CancellationToken ct);
}
