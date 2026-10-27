using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventHandlers;

public class CreateEventDTO
{
	[Required]
	public string HostId { get; set; }
	[Required]
	public int GameId { get; set; }
	[Required]
	[MaxLength(100)]
	public string Title { get; set; }
	[MaxLength(1000)]
	public string? Description { get; set; }
	public DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public int MaxPlayers { get; set; }
	public DateTime CreatedAt { get; set; }

}
