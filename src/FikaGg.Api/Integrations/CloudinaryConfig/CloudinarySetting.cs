namespace FikaGg.Api.Integrations.CloudinaryConfig;

public class CloudinarySetting
{
	public const string SectionName = "Cloudinary";

	public string CloudName { get; set; } = string.Empty;
	public string ApiKey { get; set; } = string.Empty;
	public string ApiSecret { get; set ; } = string.Empty;

	// Change to fikagg/prod when deploying to production
	public string RootFolder { get; set; } = "fikagg/dev";
}
