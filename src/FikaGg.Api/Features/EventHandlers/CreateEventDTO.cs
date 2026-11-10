using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventHandlers;

public record CreateEventDTO(
	[Required, MaxLength(100)] string Title,
	[MaxLength(1000)] string? Description,
	DateTime StartTime,
	DateTime EndTime,
	int MaxPlayers,
	DateTime CreatedAt,
	// The only reason game is a separate DTO is because of future proofing; in the
	// future, we might want an event to be able to host multiple games.
	CreateEventGameDTO EventGame
);
