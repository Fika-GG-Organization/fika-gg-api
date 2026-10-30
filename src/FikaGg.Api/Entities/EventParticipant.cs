using FikaGg.Api.Common.Enums;

namespace FikaGg.Api.Entities;

public class EventParticipant
{
    public int EventId { get; set; }
    public string UserId { get; set; } = "";
    public ParticipantStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }

	// Relations

	/// <summary> A <b>Navigation Property</b> that points to the Event associated with this User. </summary>
	public virtual Event? Event { get; set; }

	/// <summary> A <b>Navigation Property</b> that points to the User associated with this Event. </summary>
	public virtual ApplicationUser? User { get; set; }
}
