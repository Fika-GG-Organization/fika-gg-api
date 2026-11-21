namespace FikaGg.Api.Features.EventHandlers;

public class EventHostMigrateDTO
{
	public string TargetUserEmail { get; set; }
	public string TargetUserDisplayName { get; set; }
	public int EventId { get; set; }
}
