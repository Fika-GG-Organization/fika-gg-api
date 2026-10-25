namespace FikaGg.Api.Features.Events;

public record EventGetIdDTO(
	int Id,
	string Title,
	DateTime StartTime,
	DateTime EndTime,
	string? Description,
	int? GameId,
	string? CoverUrl,
	int? ParticipantCount,
	int? MaxPlayers,
	List<EventParticipantDTO> Players
);
