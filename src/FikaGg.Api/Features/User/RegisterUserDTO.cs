using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.User;

public class RegisterUserDTO
{
	[Required]
	[MaxLength(50)]
	public string DisplayName { get; set; } = "";

	[Required]
	[EmailAddress(ErrorMessage = "Invalid email format")]
	public string Email { get; set; }

	[Required(ErrorMessage = "Password is required")]
	[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
	[RegularExpression(
			@"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$",
			ErrorMessage = "Password must contain at least one uppercase letter, one number and one special character.")]
	public string Password { get; set; }
}
