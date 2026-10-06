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
    public ApplicationUser Host { get; set; } = null!;
    public Game Game { get; set; } = null!;
    public List<EventParticipant> Participants { get; set; } = [];
}
