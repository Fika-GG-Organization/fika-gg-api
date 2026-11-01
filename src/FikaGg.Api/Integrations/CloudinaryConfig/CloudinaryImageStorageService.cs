using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace FikaGg.Api.Integrations.CloudinaryConfig;

public class CloudinaryImageStorageService : IImageStorageService
{
	private readonly Cloudinary _cloudinary;

	public CloudinaryImageStorageService(IOptions<CloudinarySetting> options)
	{
		var s = options.Value;
		_cloudinary = new Cloudinary(new Account(s.CloudName, s.ApiKey, s.ApiSecret));
		_cloudinary.Api.Secure = true;
	}

	public async Task<UploadedImage> UploadProfilePictureAsync(Stream stream, string fileName, CancellationToken ct)
	{
		var uploadParams = new ImageUploadParams
		{
			File = new FileDescription(fileName, stream),
			Folder = "profile-pictures",
			Transformation = new Transformation()
				.Width(400)
				.Height(400)
				.Crop("fill")
				.Gravity("face")
		};

		var result = await _cloudinary.UploadAsync(uploadParams, ct);

		if (result.Error is not null)
		{
			throw new InvalidOperationException($"Cloudinary failed to upload: {result.Error.Message}");
		}

		return new UploadedImage(result.SecureUrl.ToString(), result.PublicId);
	}

	public async Task DeleteAsync(string publicId, CancellationToken ct)
	{
		var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId)
		{
			Invalidate = true
		});

		if (result.Error is not null)
		{
			throw new InvalidOperationException($"Cloudinary failed to delete: {result.Error.Message}");
		}
	}
}
