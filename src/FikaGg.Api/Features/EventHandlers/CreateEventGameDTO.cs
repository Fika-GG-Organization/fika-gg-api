using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventHandlers;

/// <summary>
/// A DTO that will be sent alongside <see cref="CreateEventDTO"/>.
/// It is used to add a Game to the database if it does not already exist.
/// </summary>
public record CreateEventGameDTO(
	int? Id,
	int? RawgId,
	[Required, MaxLength(100)] string Name,
	[MaxLength(500)] string? CoverUrl
);
