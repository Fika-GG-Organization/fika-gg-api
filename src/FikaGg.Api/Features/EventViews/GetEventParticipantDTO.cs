using FikaGg.Api.Common.Enums;
using FikaGg.Api.Entities;
using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventViews;

public record GetEventParticipantDTO(
	[Required] int EventId,
	[Required] string UserId,
	ParticipantStatus Status,
	[Required] DateTime JoinedAt
);
