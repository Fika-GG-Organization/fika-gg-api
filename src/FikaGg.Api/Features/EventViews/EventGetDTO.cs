namespace FikaGg.Api.Features.Events;

public record EventGetDTO(
	int Id,
	string Title,
	DateTime EventStart,
	DateTime EventEnd,
	string? Description,
	int? GameId,
	int? ParticipantCount,
	int? MaxPlayers
);
