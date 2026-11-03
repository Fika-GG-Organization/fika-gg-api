using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.EventHandlers;
using FikaGg.Api.Features.EventViews;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.EventHandlers;

public class EventHandlerService(AppDbContext context, IEventViewsService eventViewService) : IEventHandlerService
{
	public async Task<GetEventByIdDTO> CreateEventAsync(CreateEventDTO createEventDto, string userId)
	{
		Validate.CreateEventDto(createEventDto);

		var game = await context.Games.FirstOrDefaultAsync(
			g => g.RawgId == createEventDto.EventGame.RawgId);

		if (game == null)
		{
			// If a game could not be found, add it to dbcontext.
			game = context.Games.Add(new()
			{
				RawgId = createEventDto.EventGame.RawgId,
				Name = createEventDto.EventGame.Name,
				CoverUrl = createEventDto.EventGame.CoverUrl,
			})
				.Entity;
			// Return the newly added game to 'game' to make it not null.
		}

		var result = await context.Events.AddAsync(new()
		{
			HostId = userId,
			GameId = game.Id,
			Title = createEventDto.Title,
			Description = createEventDto.Description,
			StartTime = createEventDto.StartTime,
			EndTime = createEventDto.EndTime,
			MaxPlayers = createEventDto.MaxPlayers,
			CreatedAt = createEventDto.CreatedAt,
		});

		await context.SaveChangesAsync();

		return await eventViewService.GetEventByIdAsync(result.Entity.Id)
			?? throw new InvalidOperationException("Could not find event despite it being created.");
	}
	}

		await context.SaveChangesAsync();
		return result.Entity;
	}
	public Task<Event> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto, string userId) => throw new NotImplementedException();
	public Task<Event> DeleteEventAsync(int eventId, string userId) => throw new NotImplementedException();
}
