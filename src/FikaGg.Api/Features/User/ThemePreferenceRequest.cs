using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.User;

public sealed class ThemePreferenceRequest
{
	[Required]
	[RegularExpression("^(light|dark)$")]
	public string ThemePreference { get; set; } = "";
}
