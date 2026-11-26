using FikaGg.Api.Data;
using FikaGg.Api.Features.EventViews;
using FikaGg.Api.Features.JoinEvent;
using Microsoft.EntityFrameworkCore;

namespace FikaGg.Tests.Features.JoinEvent;

/// <summary>
/// Author: Alex Coh
/// </summary>
public class JoinEventServiceTests
{
	/// <summary>
	/// <see cref="ITestOutputHelper"/> should be used to write to Test Detail Summary output.<br /><br />
	/// <see cref="Console.WriteLine()"/> does not work in xUnit test methods.
	/// </summary>
	private readonly ITestOutputHelper _output;
	private readonly AppDbContext _db;
	private readonly JoinEventService _joinService;

	public JoinEventServiceTests(ITestOutputHelper output)
	{
		_output = output;

		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

		_db = new AppDbContext(options);

		_joinService = new JoinEventService(_db, new EventViewsService(_db));
	}

	private void SeedInMemoryData()
	{
		_db.Users.AddRange([
			new() { Id = "user-1", UserName = "user-1" },
			new() { Id = "user-2", UserName = "user-2" },
		]);

		_db.Games.Add(new()
		{
			Id = 1,
			Name = "Super Mario Bros.",
		});

		_db.SaveChanges();

		_db.Events.AddRange([
			new() {
				Id = 1,
				HostId = "user-1",
				GameId = 1,
				Title = "This is 'Event 1'",
				StartTime = new DateTime(2027, 12, 24, 12, 00, 00, DateTimeKind.Utc),
				EndTime = new DateTime(2027, 12, 24, 14, 00, 00, DateTimeKind.Utc),
				MaxPlayers = 10,
				CreatedAt = DateTime.UtcNow,
			},
		]);

		_db.SaveChanges();
	}

	[Fact]
	public async Task EventTime_OverlapsWithOtherEvents_ReturnsTrue()
	{
		// Arrange
		this.SeedInMemoryData();

		_db.Events.AddRange([
			new() {
				Id = 11,
				HostId = "user-1",
				GameId = 1,
				Title = "Event 11 begins one hour after Event 1 starts.",
				StartTime = new DateTime(2027, 12, 24, 13, 00, 00, DateTimeKind.Utc),
				EndTime = new DateTime(2027, 12, 24, 15, 00, 00, DateTimeKind.Utc),
				MaxPlayers = 10,
				CreatedAt = DateTime.UtcNow,
			},
			new() {
				Id = 12,
				HostId = "user-1",
				GameId = 1,
				Title = "Event 12 begins ends hour after Event 1 starts.",
				StartTime = new DateTime(2027, 12, 24, 11, 00, 00, DateTimeKind.Utc),
				EndTime = new DateTime(2027, 12, 24, 13, 00, 00, DateTimeKind.Utc),
				MaxPlayers = 10,
				CreatedAt = DateTime.UtcNow,
			}
		]);


		// Act
		await _joinService.AddEventParticipantAsync("user-2", 1);
		var result1 = await _joinService.CheckIfEventsOverlapAsync("user-2", 11);
		var result2 = await _joinService.CheckIfEventsOverlapAsync("user-2", 12);

		// Assert
		Assert.True(result1.IsOverlapping);
		Assert.True(result2.IsOverlapping);
	}

	[Fact]
	public async Task AnEventEnds_AnotherEventStartsAtTheSame_TheyCountAsOverlapping()
	{
		// Arrange
		this.SeedInMemoryData();

		_db.Events.AddRange([
			new()
			{
				Id = 21,
				HostId = "user-1",
				GameId = 1,
				Title = "Event 21 begins exactly as event 1 ends!",
				StartTime = new DateTime(2027, 12, 24, 14, 00, 00, DateTimeKind.Utc),
				EndTime = new DateTime(2027, 12, 24, 16, 00, 00, DateTimeKind.Utc),
				MaxPlayers = 10,
				CreatedAt = DateTime.UtcNow,
			},
			new()
			{
				Id = 22,
				HostId = "user-1",
				GameId = 1,
				Title = "Event 22 begins one second after event 1 ends!",
				StartTime = new DateTime(2027, 12, 24, 14, 00, 01, DateTimeKind.Utc),
				EndTime = new DateTime(2027, 12, 24, 16, 00, 00, DateTimeKind.Utc),
				MaxPlayers = 10,
				CreatedAt = DateTime.UtcNow,
			}
		]);

		// Act
		await _joinService.AddEventParticipantAsync("user-2", 1);
		var result1 = await _joinService.CheckIfEventsOverlapAsync("user-2", 21);
		var result2 = await _joinService.CheckIfEventsOverlapAsync("user-2", 22);

		// Assert
		Assert.True(result1.IsOverlapping);
		Assert.False(result2.IsOverlapping);
	}

	[Fact]
	public async Task NoOverlappingEvents_WhenAttemptingToJoin_ReturnFalse()
	{
		// Arrange
		this.SeedInMemoryData();

		_db.Events.Add(new()
		{
			Id = 31,
			HostId = "user-1",
			GameId = 1,
			Title = "Event 31 begins a year after event 1!",
			StartTime = new DateTime(2028, 12, 24, 12, 00, 00, DateTimeKind.Utc),
			EndTime = new DateTime(2028, 12, 24, 14, 00, 00, DateTimeKind.Utc),
			MaxPlayers = 10,
			CreatedAt = DateTime.UtcNow,
		});


		// Act
		await _joinService.AddEventParticipantAsync("user-2", 1);
		var result = await _joinService.CheckIfEventsOverlapAsync("user-2", 31);

		// Assert
		Assert.False(result.IsOverlapping);
	}

}
