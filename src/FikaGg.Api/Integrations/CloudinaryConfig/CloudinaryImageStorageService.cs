using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace FikaGg.Api.Integrations.CloudinaryConfig;

public class CloudinaryImageStorageService : IImageStorageService
{
	private readonly Cloudinary _cloudinary;
	private readonly CloudinarySetting _setting;

	public CloudinaryImageStorageService(IOptions<CloudinarySetting> options)
	{
		_setting = options.Value;
		_cloudinary = new Cloudinary(new Account(_setting.CloudName, _setting.ApiKey, _setting.ApiSecret));
		_cloudinary.Api.Secure = true;
	}

	public async Task<UploadedImage> UploadAvatarAsync(Stream stream, string fileName, string userId, CancellationToken ct)
	{
		var uploadParams = new ImageUploadParams
		{
			// Resizing and cropping to 400x400 with face detection, so that the avatar is always square and centered
			File = new FileDescription(fileName, stream),
			Folder = $"{_setting.RootFolder.Trim('/')}/avatars/{userId}",
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
