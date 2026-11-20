using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.User;

public class UpdateProfileDTO
{
	[MaxLength(50)]
	public string? DisplayName { get; set; }

	[EmailAddress(ErrorMessage = "Invalid email format")]
	public string? Email { get; set; }

	public string? CurrentPassword { get; set; }

	[StringLength(100)]
	[RegularExpression(
		@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
		ErrorMessage = "Password must contain at least one uppercase letter, one number and one special character.")]
	public string? NewPassword { get; set; }
}
