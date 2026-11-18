using FikaGg.Api.Integrations.Rawg;

namespace FikaGg.Api.Features.Game;

public class GameService(IRawgClient rawgClient)
{
	public async Task<List<GameSearchResultDTO>> SearchAsync(string query)
	{
		query = query.Trim();

		// dont spend requests on searches that are too short, limited quota of 20k requests per month
		if (query.Length < 2)
		{
			return [];
		}

		var games = await rawgClient.SearchGamesAsync(query);

		return games
			.Select(g => new GameSearchResultDTO(g.Id, g.Name, g.BackgroundImage))
			.ToList();
	}
}
