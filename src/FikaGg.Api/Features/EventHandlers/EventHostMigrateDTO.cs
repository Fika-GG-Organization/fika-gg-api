using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.EventHandlers;

public class EventHostMigrateDTO
{
	[Required]
	[EmailAddress(ErrorMessage ="Invalid Email format")]
	public string TargetUserEmail { get; set; }

	[Required]
	[MaxLength(50)]
	public string TargetUserDisplayName { get; set; }

	[Required]
	public int EventId { get; set; }
}
