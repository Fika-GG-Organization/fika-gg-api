namespace FikaGg.Api.Integrations.Rawg;

public interface IRawgClient
{
	Task<List<RawgGame>> SearchGamesAsync(string query);

	Task <RawgGame> GetGameDetailsAsync(int gameId);
}
