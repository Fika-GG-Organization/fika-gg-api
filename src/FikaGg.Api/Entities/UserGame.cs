using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class UserGame
{
    public string UserId { get; set; } = "";
    public int GameId { get; set; }
    [MaxLength(50)] public string? GamerTag { get; set; }

	// Relations

	/// <summary> A <b>Navigation Property</b> that points to the User associated with this Game. </summary>
	public virtual ApplicationUser? User { get; set; }

	/// <summary> A <b>Navigation Property</b> that points to the Game associated with this User. </summary>
	public virtual Game? Game { get; set; }
}
