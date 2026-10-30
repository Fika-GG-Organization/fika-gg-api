using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Entities;

public class ApplicationUser : IdentityUser
{
    [MaxLength(50)] public string DisplayName { get; set; } = "";
    [MaxLength(500)] public string? AvatarUrl { get; set; }

	// Relations

	/// <summary> A <b>Navigation Property</b> that points to the list of Events hosted by this User. </summary>
	public virtual ICollection<Event> HostedEvents { get; set; } = [];

	/// <summary> A <b>Navigation Property</b> that points to the list of Events this User participates in. </summary>
	public virtual ICollection<EventParticipant> JoinedEvents { get; set; } = [];

	/// <summary> A <b>Navigation Property</b> that points to the list of Games in this User's profile. </summary>
	public virtual ICollection<UserGame> Games { get; set; } = [];

}
