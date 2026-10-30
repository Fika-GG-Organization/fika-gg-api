using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class Game
{
    public int Id { get; set; }
    public int? RawgId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(500)] public string? CoverUrl { get; set; }

	// Relations

	/// <summary> List of Users that has added this Game to their profile. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ICollection<UserGame> UserGames { get; set; } = [];

	/// <summary> List of Events that wants to play this Game. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ICollection<Event> Events { get; set; } = [];
}
