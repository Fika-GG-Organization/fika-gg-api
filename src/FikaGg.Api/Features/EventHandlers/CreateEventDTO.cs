using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventHandlers;

public record CreateEventDTO(
	[Required] int GameId,
	[Required, MaxLength(100)] string Title,
	[MaxLength(1000)] string? Description,
	DateTime StartTime,
	DateTime EndTime,
	int MaxPlayers,
	DateTime CreatedAt
);
