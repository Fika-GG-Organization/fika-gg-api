namespace FikaGg.Api.Integrations.CloudinaryConfig;

public interface IImageStorageService
{
	Task<UploadedImage> UploadProfilePictureAsync(Stream stream, string fileName, CancellationToken ct);
	Task DeleteAsync(string publicId, CancellationToken ct);
}
