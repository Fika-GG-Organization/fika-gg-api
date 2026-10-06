using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class UserGame
{
    public string UserId { get; set; } = "";
    public int GameId { get; set; }
    [MaxLength(50)] public string? GamerTag { get; set; }
    
    // Relations
    public ApplicationUser User { get; set; } = null!;
    public Game Game { get; set; } = null!;
}
