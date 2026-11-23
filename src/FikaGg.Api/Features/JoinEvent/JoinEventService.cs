using FikaGg.Api.Common.Enums;
using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Features.EventViews;
using FikaGg.Api.Features.Notices;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Api.Features.JoinEvent;

public class JoinEventService(AppDbContext dbContext, IEventViewsService viewService)
{
	public async Task<GetEventByIdDTO> AddEventParticipantAsync(string userId, int eventId)
	{
		// event is needed to get participant count and max player limit.
		var @event = await dbContext.Events
			.Where(e => e.Id == eventId)
			.Include(e => e.Participants.OrderBy(p => p.JoinedAt))
			.FirstOrDefaultAsync()
			?? throw new KeyNotFoundException($"Could not find Event with that id.");

		var eventParticipant = @event.Participants.FirstOrDefault(p => p.UserId == userId);

		if (eventParticipant == null)
		{
			dbContext.EventParticipants.Add(new()
			{
				UserId = userId,
				EventId = eventId,
				Status = (@event.Participants.Count < @event.MaxPlayers)
					? ParticipantStatus.Joined   // User will join if max player limit is not reached.
					: ParticipantStatus.Backup,  // Otherwise user will join as backup.
				JoinedAt = Converter.DateConverter(DateTime.UtcNow.ToString("o")),
			});

			dbContext.SaveChanges();
		}
		else
		{
			throw new ArgumentException("User is already participating in this event.");
		}

		return await viewService.GetEventByIdAsync(eventId)
			?? throw new InvalidOperationException("Could not find event despite modifying it.");
	}

	public async Task<GetEventByIdDTO> RemoveEventParticipantAsync(string userId, int eventId)
	{
		// Prevent user to leave the event if they're the host
		var @event = await dbContext.Events
			.Where(e => e.Id == eventId).Include(e => e.Participants).FirstOrDefaultAsync()
			?? throw new KeyNotFoundException($"Could not find the event to modify.");

		if (@event.HostId == userId)
		{
			throw new ArgumentException($"The user is the host of the event and can not therefor remove themself from UserParticipant list.");
		}

		// Remove user from the event's participant list
		var eventParticipant = @event.Participants.FirstOrDefault(p => p.UserId == userId)
			?? throw new KeyNotFoundException($"Could not find the user to remove from event participation list.");

		dbContext.EventParticipants.Remove(eventParticipant);

		var remaining = @event.Participants
			.Where(p => p.UserId != userId)
			.Where(p => p.Status is ParticipantStatus.Joined or ParticipantStatus.Backup)
			.OrderBy(p => p.JoinedAt)
			.ToList();

		foreach(var player in remaining.Take(@event.MaxPlayers).Where(p => p.Status == ParticipantStatus.Backup))
		{
			player.Status = ParticipantStatus.Joined;
			dbContext.Notices.Add(NoticeTemplates.PromotedFromReserve(player.UserId, @event.Title));
		}

		await dbContext.SaveChangesAsync();
		
		return await viewService.GetEventByIdAsync(eventId)
			?? throw new InvalidOperationException("Could not find event despite modifying it.");
	}

	/// <summary>
	/// Iterates through every event the user is participating in and checks if any of them overlap
	/// with the requested event.
	/// </summary>
	/// <param name="userId">Identifies all events the user is an participant of.</param>
	/// <param name="eventId">Identifies the requested event, i.e. the event user is trying to join.</param>
	/// <returns>
	/// <see langword="true"/> if there are overlaps with the requested event; otherwise return <see langword="false"/>.
	/// </returns>
	public async Task<DTOGetCheck> CheckIfEventsOverlapAsync(string userId, int eventId)
	{
		var @event = await dbContext.Events.FindAsync(eventId)
			?? throw new KeyNotFoundException("Could not find the event user is trying to join.");

		await foreach (var ep in dbContext.EventParticipants
			.Where(ep => ep.UserId == userId && ep.EventId != eventId)
			.Include(ep => ep.Event)
			.AsAsyncEnumerable())
		{
			if (ep.Event != null)
			{
				// Source - https://stackoverflow.com/a/13513973
				// Posted by Rawling
				// Retrieved 2026-11-22, License - CC BY-SA 3.0
				if (ep.Event.StartTime <= @event.EndTime && @event.StartTime <= ep.Event.EndTime)
				{
					return new DTOGetCheck(true);
				}
			}
		}
		return new DTOGetCheck(false);
	}
}
