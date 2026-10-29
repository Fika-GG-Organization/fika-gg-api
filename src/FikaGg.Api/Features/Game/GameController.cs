using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FikaGg.Api.Features.Game;

[ApiController]
[Route("api/games")]
[Authorize]
public class GameController(GameService gameService) : ControllerBase
{
	[HttpGet("search")]
	public async Task<ActionResult<List<GameSearchResultDTO>>> Search(
		[FromQuery, Required, MaxLength(100)] string query)
	{
		var results = await gameService.SearchAsync(query);
		return Ok(results);
	}
}
