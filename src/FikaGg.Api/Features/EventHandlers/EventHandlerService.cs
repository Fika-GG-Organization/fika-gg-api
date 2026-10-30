using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.EventHandlers;

public class EventHandlerService(AppDbContext context) : IEventHandlerService
{
	public async Task<Event> CreateEventAsync(CreateEventDTO createEventDto, string userId)
	{
		Validate.CreateEventDto(createEventDto);

		var result = await context.Events.AddAsync(new()
		{
			HostId = userId,
			GameId = createEventDto.GameId,
			Title = createEventDto.Title,
			Description = createEventDto.Description,
			StartTime = createEventDto.StartTime,
			EndTime = createEventDto.EndTime,
			MaxPlayers = createEventDto.MaxPlayers,
			CreatedAt = createEventDto.CreatedAt,
		});
		await context.SaveChangesAsync();
		return result.Entity;
	}
	public Task<Event> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto, string userId) => throw new NotImplementedException();
	public Task<Event> DeleteEventAsync(int eventId, string userId) => throw new NotImplementedException();
}
