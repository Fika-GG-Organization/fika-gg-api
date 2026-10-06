using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class Game
{
    public int Id { get; set; }
    public int? RawgId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(500)] public string? CoverUrl { get; set; }
}
