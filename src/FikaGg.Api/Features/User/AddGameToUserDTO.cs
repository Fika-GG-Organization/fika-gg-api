using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.User;

public class AddGameToUserDTO
{
	[Required]
	public int GameId { get; set; }
}
