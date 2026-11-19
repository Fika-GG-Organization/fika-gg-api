namespace FikaGg.Api.Features.EventViews;

public record GetEventHostDTO(
	string Id,
	string DisplayName,
	string? AvatarUrl
	);
