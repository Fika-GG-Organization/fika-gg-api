namespace FikaGg.Api.Integrations.CloudinaryConfig;

public static class CloudinaryServiceExtension
{
	public static IServiceCollection AddCloudinary(this IServiceCollection services, IConfiguration config)
	{
		services
			.AddOptions<CloudinarySetting>()
			.Bind(config.GetSection(CloudinarySetting.SectionName))
			.Validate(s =>
				!string.IsNullOrWhiteSpace(s.CloudName)
				&& !string.IsNullOrWhiteSpace(s.ApiKey)
				&& !string.IsNullOrWhiteSpace(s.ApiSecret),
				"Cloudinary settings are missing.")
			.ValidateOnStart();

		services.AddSingleton<IImageStorageService, CloudinaryImageStorageService>();

		return services;
	}
}
