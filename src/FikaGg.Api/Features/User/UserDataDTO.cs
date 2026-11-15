using FikaGg.Api.Features.Events;
using FikaGg.Api.Features.Game;

namespace FikaGg.Api.Features.User;

public record UserDataDTO(
	string Id,
	string DisplayName,
	string? AvatarUrl,
	List<EventGetDTO> UserEvents,
	List<EventGetDTO> HostedEvents,
	List<GameSearchResultDTO> OwnedGames

);

