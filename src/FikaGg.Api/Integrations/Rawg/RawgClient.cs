namespace FikaGg.Api.Integrations.Rawg;

public class RawgClient(HttpClient http, IConfiguration config) : IRawgClient
{
	public async Task<List<RawgGame>> SearchGamesAsync(string query)
	{
		var apiKey = config["Rawg:ApiKey"]
			?? throw new InvalidOperationException("Rawg:ApiKey is missing");

		var url = $"api/games?search={Uri.EscapeDataString(query)}&page_size=10&key={apiKey}";
		var response = await http.GetFromJsonAsync<RawgSearchResponse>(url);

		return response?.Results ?? [];
	}
}
