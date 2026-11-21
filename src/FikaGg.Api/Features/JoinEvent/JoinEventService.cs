using FikaGg.Api.Common.Enums;
using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Features.EventViews;
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

		dbContext.SaveChanges();

		// Then all event participants status must be updated.
		var list = await dbContext.Events
			.Where(e => e.Id == eventId)
			.Include(e => e.Participants.OrderBy(e => e.JoinedAt))
			.FirstOrDefaultAsync();

		if (list != null)
		{
			// All players within limit will join.
			await foreach (var p in list.Participants.Take(list.MaxPlayers).ToAsyncEnumerable())
			{
				p.Status = ParticipantStatus.Joined;
			}
			// All players outside of limit will become backup.
			await foreach (var p in list.Participants.Skip(list.MaxPlayers).ToAsyncEnumerable())
			{
				p.Status = ParticipantStatus.Backup;
			}
		}

		dbContext.SaveChanges();

		return await viewService.GetEventByIdAsync(eventId)
			?? throw new InvalidOperationException("Could not find event despite modifying it.");
	}
}
