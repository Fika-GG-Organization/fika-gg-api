namespace FikaGg.Api.Features.EventViews;

public record GetEventGameDTO(
	int Id,
	int? RawgId,
	string Name,
	string? CoverUrl
);
