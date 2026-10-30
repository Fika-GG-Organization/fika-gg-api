using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class Event
{
    public int Id { get; set; }
    public string HostId { get; set; } = "";
    public int GameId { get; set; }
    [MaxLength(100)] public string Title { get; set; } = "";
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int MaxPlayers { get; set; }
    public DateTime CreatedAt { get; set; }

	// Relations

	/// <summary> A <b>Navigation Property</b> that points to the User that hosts this Event. </summary>
	public virtual ApplicationUser? Host { get; set; }

	/// <summary> A Navigation Property that points to the Game that will be played at this Event. </summary>
	public virtual Game? Game { get; set; }

	/// <summary> A <b>Navigation Property</b> that points to the list of Users who wants to participate in this Event. </summary>
	public virtual ICollection<EventParticipant> Participants { get; set; } = [];
}
