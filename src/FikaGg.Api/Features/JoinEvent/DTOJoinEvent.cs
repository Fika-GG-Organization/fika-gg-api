namespace FikaGg.Api.Features.JoinEvent;

/// <summary>
/// DTO for adding an user to an event. The user id should be acquired outside of this DTO.
/// </summary>
/// <param name="EventId">Identifies the event the user will be added to as a participant.</param>
/// <param name="Status">An integer that maps to <c><see cref="Common.Enums.ParticipantStatus"/></c>.</param>
/// <param name="JoinedAt">A string in the ISO 8601 format, i.e: <c>1992-12-30T23:59:59:999Z</c></param>
public record DTOJoinEvent(
	int EventId,
	int Status,
	string JoinedAt
	);
