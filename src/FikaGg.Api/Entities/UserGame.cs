using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class UserGame
{
    public string UserId { get; set; } = "";
    public int GameId { get; set; }
    [MaxLength(50)] public string? GamerTag { get; set; }
    
    // Relations

	/// <summary> Connects User with Game. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ApplicationUser? User { get; set; }

	/// <summary> Connects Game with User. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual Game? Game { get; set; }
}
