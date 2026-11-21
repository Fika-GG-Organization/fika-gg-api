using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventHandlers;

public class EventHostMigrateDTO
{
	[Required]
	public string TargetUserId { get; set; } = null!;

	[Required]
	public int EventId { get; set; }
}
