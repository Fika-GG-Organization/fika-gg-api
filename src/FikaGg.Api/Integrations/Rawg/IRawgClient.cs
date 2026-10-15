namespace FikaGg.Api.Integrations.Rawg;

public interface IRawgClient
{
	Task<List<RawgGame>> SearchGamesAsync(string query);
}
