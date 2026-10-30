using FikaGg.Api.Common.Enums;

namespace FikaGg.Api.Entities;

public class EventParticipant
{
    public int EventId { get; set; }
    public string UserId { get; set; } = "";
    public ParticipantStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }

	// Relations

	/// <summary> Connects Event with User. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual Event? Event { get; set; }

	/// <summary> Connects User with Event. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ApplicationUser? User { get; set; }
}
