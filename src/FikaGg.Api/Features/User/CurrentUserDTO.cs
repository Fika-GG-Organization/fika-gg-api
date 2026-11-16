namespace FikaGg.Api.Features.User;

// This DTO is strictly for authentication purposes
public record CurrentUserDTO(
	string Id,
	string Email,
	string DisplayName,
	string? AvatarUrl
);
