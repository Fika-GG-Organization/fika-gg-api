using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventViews;

public record GetEventDTO(
	int Id,
	string HostId,
	string Title,
	string? Description,
	int? GameId,
	DateTime StartTime,
	DateTime EndTime,
	string? CoverUrl,
	int? ParticipantCount,
	int? MaxPlayers,
	DateTime CreatedAt
);
