namespace FikaGg.Api.Features.EventHandlers;

public static class Validate
{
	public static void CreateEventDto(CreateEventDTO dto)
	{
		if (dto.StartTime >= dto.EndTime)
		{
			throw new ArgumentException($"Event cannot start before it ends.");
		}
		if (dto.CreatedAt >= dto.StartTime)
		{
			throw new ArgumentException($"Event cannot be created after it has started.");
		}
		if (dto.MaxPlayers < 1)
		{
			throw new ArgumentException($"Max player count must be at least 1.");
		}
	}
}
