using System.Security.Cryptography.X509Certificates;

namespace FikaGg.Api.Features.EventHandlers;

public record EventCreateDTO(
	string HostId,
	int GameId,
	string Title,
	string? Description,
	string StartTime,
	string EndTime,
	int MaxPlayers,
	string CreatedAt
);
