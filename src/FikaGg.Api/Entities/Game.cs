using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class Game
{
    public int Id { get; set; }
    public int? RawgId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(500)] public string? CoverUrl { get; set; }

	// Relations

	/// <summary> A <b>Navigation Property</b> that points to the list of Users that has this Game in their profile. </summary>
	public virtual ICollection<UserGame> UserGames { get; set; } = [];

	/// <summary> A <b>Navigation Property</b> that points to the list of Events that wants to play this Game. </summary>
	public virtual ICollection<Event> Events { get; set; } = [];
}
