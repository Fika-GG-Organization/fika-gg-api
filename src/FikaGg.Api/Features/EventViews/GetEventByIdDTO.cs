using System.ComponentModel.DataAnnotations;
using FikaGg.Api.Features.EventViews;

namespace FikaGg.Api.Features.EventViews;

public record GetEventByIdDTO(
	int Id,
	string HostId,
	string Title,
	string? Description,
	GetEventGameDTO? Game,
	DateTime StartTime,
	DateTime EndTime,
	int? ParticipantCount,
	int? MaxPlayers,
	List<GetEventParticipantDTO> Players,
	DateTime CreatedAt
);
