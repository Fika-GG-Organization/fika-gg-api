using FikaGg.Api.Common.Enums;
using FikaGg.Api.Data;
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

	public async Task<GetEventByIdDTO> UpdateEventAsync(int eventId, CreateEventDTO updateEventDto, string userId)
	{
		var @event = await context.Events.FirstOrDefaultAsync(e => e.Id == eventId)
			?? throw new KeyNotFoundException();

		if (@event?.HostId != userId)
		{
			throw new ArgumentException("User does not own this event.");
		}

		var game = await context.Games
			.FirstOrDefaultAsync(g => g.RawgId == updateEventDto.EventGame.RawgId)
			?? context.Games.Add(new()
			{
				RawgId = updateEventDto.EventGame.RawgId,
				Name = updateEventDto.EventGame.Name,
				CoverUrl = updateEventDto.EventGame.CoverUrl,
			})
			.Entity;

		if (@event == null)
		{
			var result = await context.Events.AddAsync(new()
			{
				Id = eventId,
				HostId = userId,
				GameId = game.Id,
				Title = updateEventDto.Title,
				Description = updateEventDto.Description,
				StartTime = updateEventDto.StartTime,
				EndTime = updateEventDto.EndTime,
				MaxPlayers = updateEventDto.MaxPlayers,
				CreatedAt = updateEventDto.CreatedAt,
			});
		}
		else
		{
			@event.Id = eventId;
			@event.HostId = userId;
			@event.GameId = game.Id;
			@event.Title = updateEventDto.Title;
			@event.Description = updateEventDto.Description;
			@event.StartTime = updateEventDto.StartTime;
			@event.EndTime = updateEventDto.EndTime;
			@event.MaxPlayers = updateEventDto.MaxPlayers;
			@event.CreatedAt = updateEventDto.CreatedAt;

			foreach (var player in @event.Participants.Skip(@event.MaxPlayers))
			{
				// Every participants above the MaxPlayers limit will have their status set to Backup.  
				player.Status = ParticipantStatus.Backup;
			}

			var result = context.Events.Update(@event);
		}

		await context.SaveChangesAsync();

		return await eventViewService.GetEventByIdAsync(eventId)
			?? throw new InvalidOperationException("Could not find event despite it being created.");
	}

	public async Task DeleteEventAsync(int eventId, string userId)
	{
		var @event = await context.Events
			.Include(e => e.Participants)
			.FirstOrDefaultAsync(e => e.Id == eventId)
			?? throw new KeyNotFoundException();

		if (@event.HostId != userId)
		{
			throw new ArgumentException("User does not own this event.");
		}

		if (@event != null)
		{
			context.EventParticipants.RemoveRange(@event.Participants);
			context.Events.Remove(@event);
		}

		await context.SaveChangesAsync();
	}
}
