namespace FikaGg.Api.Features.User;

public record UserDataDTO(
	string Id,
	string DisplayName,
	string? AvatarUrl,
	DateTime CreatedAt,
	List<Events> UserEvents,
	List<Events> HostedEvents,
	List<Games> OwnedGames

);

