using FikaGg.Api.Common.Enums;
using FikaGg.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventViews;

public record GetEventParticipantDTO(
	[Required] int EventId,
	[Required] string UserId,
	[Required] string DisplayName,
	string? AvatarUrl,
	ParticipantStatus Status,
	[Required] DateTime JoinedAt
);
