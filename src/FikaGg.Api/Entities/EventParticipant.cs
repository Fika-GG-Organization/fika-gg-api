using FikaGg.Api.Common.Enums;

namespace FikaGg.Api.Entities;

public class EventParticipant
{
    public int EventId { get; set; }
    public string UserId { get; set; } = "";
    public ParticipantStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }

    public Event Event { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
