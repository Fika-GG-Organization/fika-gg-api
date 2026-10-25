using FikaGg.Api.Data;
using FikaGg.Api.Common.Utils;
using FikaGg.Api.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FikaGg.Api.Features.EventHandlers;

public class EventService(AppDbContext dbContext)
{
	public async Task<EventCreateDTO> CreateEventAsync(EventCreateDTO dto, string UserId)
	{
		var newEvent = new Event
		{
			HostId = dto.HostId,
			GameId = dto.GameId,
			Title = dto.Title,
			Description = dto.Description,
			StartTime = Converter.DateConverter(dto.StartTime),
			EndTime = Converter.DateConverter(dto.EndTime),
			MaxPlayers = dto.MaxPlayers,
			CreatedAt = Converter.DateConverter(dto.CreatedAt),
		};

		await dbContext.AddAsync(newEvent);
		await dbContext.SaveChangesAsync();

		
	}
}
