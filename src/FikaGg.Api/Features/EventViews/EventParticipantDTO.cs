namespace FikaGg.Api.Features.Events;

public record EventParticipantDTO(
	string Id,
	string DisplayName,
	string? AvatarUrl,
	string? Status,
	DateTime? JoinedAt
);
