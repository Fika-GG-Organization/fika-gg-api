using FikaGg.Api.Common.Enums;
using FikaGg.Api.Common.Utils;
using FikaGg.Api.Data;
using FikaGg.Api.Features.EventViews;

namespace FikaGg.Api.Features.JoinEvent;

public class JoinEventService(AppDbContext dbContext, IEventViewsService viewService)
{
	public async Task<GetEventByIdDTO> AddUserParticipant(string userId, DTOJoinEvent dto)
	{
		Validate.DTOJoinEvent(dto);

		if (await dbContext.EventParticipants.FindAsync(dto.EventId, userId) == null)
		{
			return new Proble 
		}

		dbContext.EventParticipants.Add(new()
		{
			UserId = userId,
			EventId = dto.EventId,
			Status = (ParticipantStatus)dto.Status,
			JoinedAt = Converter.DateConverter(dto.JoinedAt),
		});

		dbContext.SaveChanges();

		return await viewService.GetEventByIdAsync(dto.EventId)
			?? throw new InvalidOperationException("Could not find event despite modifying to it.");
	}
}
