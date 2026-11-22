using System.ComponentModel.DataAnnotations;
namespace FikaGg.Api.Entities;

public class Notice
{
	public int Id { get; set; }
	public string UserId { get; set; }

	[MaxLength(100)]
	public string Title { get; set; } = "";

	[MaxLength(1000)]
	public string Message { get; set; } = "";
	public bool Viewed { get; set; } = false;
	public DateTime CreatedAt { get; set; }


	///<summary>A <b>NavigationProperty</b> that points to the User that owns this Notice</summary>
	public virtual ApplicationUser? User { get; set; }
}
