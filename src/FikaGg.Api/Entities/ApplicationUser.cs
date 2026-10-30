using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)] public string DisplayName { get; set; } = "";
    [MaxLength(500)] public string? AvatarUrl { get; set; }

	// Relations

	/// <summary> List of Events hosted by this User. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ICollection<Event> HostedEvents { get; set; } = [];

	/// <summary> List of Events this User participates in. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ICollection<EventParticipant> JoinedEvents { get; set; } = [];

	/// <summary> List of Games added to this User's profile. </summary>
	/// <remarks> Navigation Property. Should not exist in database. </remarks>
	public virtual ICollection<UserGame> Games { get; set; } = [];

}
