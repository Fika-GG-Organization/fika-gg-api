using FikaGg.Api.Features.Game;
using FikaGg.Api.Integrations.Rawg;
using static FikaGg.Tests.GetGameDetailsAsyncTests;

namespace FikaGg.Tests;

/// <summary>
/// Author: Fadi Mrito
/// </summary>
public class GameServiceTests
{
	[Fact]
	public async Task SearchAsync_QueryTooShort_ReturnsEmptyAndDoesNotCallRawg()
	{
		var fakeRawgClient = new FakeRawgClient();
		var service = new GameService(fakeRawgClient);

		var result = await service.SearchAsync("v");

		Assert.Empty(result);
		Assert.Equal(0, fakeRawgClient.CallCount);
	}

	[Fact]
	public async Task SearchAsync_ValidQuery_MapsRawgGamesToDtos()
	{
		var fakeRawgClient = new FakeRawgClient
		{
			GamesToReturn = [new RawgGame(415171, "Valorant", "https://media.rawg.io/valorant.jpg")]
		};
		var service = new GameService(fakeRawgClient);

		var result = await service.SearchAsync("valorant");

		var game = Assert.Single(result);
		Assert.Equal(415171, game.RawgId);
		Assert.Equal("Valorant", game.Name);
		Assert.Equal("https://media.rawg.io/valorant.jpg", game.CoverUrl);
	}

	[Fact]
	public async Task SearchAsync_QueryWithSpaces_TrimsBeforeSearching()
	{
		var fakeRawgClient = new FakeRawgClient();
		var service = new GameService(fakeRawgClient);

		await service.SearchAsync("  valorant  ");

		Assert.Equal("valorant", fakeRawgClient.LastQuery);
	}
}

/// <summary>
/// Author: Gabriel Kassarp
/// </summary>
public class GetGameDetailsAsyncTests
{
	[Fact]
	public async Task GetGameDetailsAsync_ReturnsGameDetails()
	{
		var fakeClient = new FakeRawgClient { GameToReturn = new RawgGame(1, "Valorant", "https://media.rawg.io/valorant.jpg") };
		var result = await fakeClient.GetGameDetailsAsync(1);
		Assert.Equal(1, result.Id);
		Assert.Equal("Valorant", result.Name);
	}

	[Fact]
	public async Task GetGameDetailsAsync_GameNotFound_Throws()
	{
		var fakeClient = new FakeRawgClient { GameToReturn = null };
		await Assert.ThrowsAsync<KeyNotFoundException>(() => fakeClient.GetGameDetailsAsync(99));
	}

	public class FakeRawgClient : IRawgClient
	{
		public List<RawgGame> GamesToReturn { get; set; } = [];
		public int CallCount { get; private set; }
		public string? LastQuery { get; private set; }
		public RawgGame? GameToReturn { get; set; }


		public Task<List<RawgGame>> SearchGamesAsync(string query)
		{
			CallCount++;
			LastQuery = query;
			return Task.FromResult(GamesToReturn);
		}

		public Task<RawgGame> GetGameDetailsAsync(int gameId)
		{
			if (GameToReturn == null)
				throw new KeyNotFoundException($"Game with ID {gameId} was not found.");
			return Task.FromResult(GameToReturn);
		}
	}
}
