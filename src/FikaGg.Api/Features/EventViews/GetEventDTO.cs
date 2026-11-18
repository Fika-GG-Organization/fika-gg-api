using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventViews;

public record GetEventDTO(
	int Id,
	string HostId,
	string Title,
	string? Description,
	GetEventGameDTO? Game,
	DateTime StartTime,
	DateTime EndTime,
	int? ParticipantCount,
	int? MaxPlayers,
	DateTime CreatedAt
);
