using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.User;
using FikaGg.Api.Features.User.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using static FikaGg.Tests.GetGameDetailsAsyncTests;

namespace FikaGg.Tests.Features.User;
public class UserServiceTests
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly UserService _userService;

	public UserServiceTests()
	{
		// Test environment creates an in-memory DB.
		// To access this DB with userManager, a userStore is setup to facilitate the communication
		// between the manager and the in-memmory-DB


		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_dbContext = new AppDbContext(options);

		var store = new UserStore<ApplicationUser>(_dbContext);

		_userManager = new UserManager<ApplicationUser>(
			store,
			Options.Create(new IdentityOptions()),
			new PasswordHasher<ApplicationUser>(),
			new[] { new UserValidator<ApplicationUser>() },
			new[] { new PasswordValidator<ApplicationUser>() },
			new UpperInvariantLookupNormalizer(),
			new IdentityErrorDescriber(),
			null!,
			NullLogger<UserManager<ApplicationUser>>.Instance);

		_userService = new UserService(_dbContext, _userManager, new FakeRawgClient());
	}


	// HELPER:

	private async Task<ApplicationUser> CreateUserAsync(
	string displayName = "Test User",
	string email = "testuser1@example.com")
	{
		var user = new ApplicationUser
		{
			DisplayName = displayName,
			Email = email,
			UserName = email
		};
		var result = await _userManager.CreateAsync(user, "TestPassword123!");
		Assert.True(result.Succeeded);
		return user;
	}

	//TESTS

	public RegisterUserDTO Generate_New_RegisterUserDTO()
	{
		return new RegisterUserDTO
		{
			DisplayName = "Test User",
			Email = "testuser1@example.com",
			Password = "TestPassword123!"
		};
	}

	public RegisterUserDTO Generate_Duplicate_DisplayName()
	{
		return new RegisterUserDTO
		{
			DisplayName = "Test User",
			Email = "testuser2@example.com",
			Password = "TestPassword123!"
		};
	}

	[Fact]
	public async Task RegisterUserAsync_ValidDto_ShouldSucceed()
	{
		// Arrange
		var dto = Generate_New_RegisterUserDTO();

		// Act
		var result = await _userService.RegisterUserAsync(dto);

		// Assert
		Assert.True(result.Succeeded);
		var created = await _userManager.FindByEmailAsync(dto.Email);
		Assert.NotNull(created);
		Assert.Equal(dto.DisplayName, created!.DisplayName);
	}

	[Fact]
	public async Task RegisterUserAsync_DuplicateDisplayName_ShouldFail()
	{
		// Arrange
		await CreateUserAsync("Test User", "existing@example.com");
		var duplicateDto = Generate_Duplicate_DisplayName();

		// Act
		var result = await _userService.RegisterUserAsync(duplicateDto);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "DuplicateDisplayName");
		Assert.Null(await _userManager.FindByEmailAsync(duplicateDto.Email));
	}

	[Fact]
	public async Task DeleteUser_Using_UserId_Should_Succeed()
	{
		// Arrange
		var user = await CreateUserAsync();
		var bystander = await CreateUserAsync("Bystander", "bystander@example.com");

		// Act
		var result = await _userService.DeleteUser(user.Id);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(await _dbContext.Users.AnyAsync(u => u.Id == user.Id));
		Assert.True(await _dbContext.Users.AnyAsync(u => u.Id == bystander.Id));
	}

	[Fact]
	public async Task Delete_Nonexisting_User_Using_UserId_Should_Fail()
	{
		// Arrange
			// NOT APPLICABLE - No prep needeed as an empty DB will yield the desired outcome
		// Act
		var result = await _userService.DeleteUser("abc");

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "UserNotFound");
	}

	[Fact]
	public async Task DeleteUser_RemovesTheirGamesParticipationsAndHostedEvents()
	{
		var host = await CreateUserAsync("Host", "host@example.com");
		var guest = await CreateUserAsync("Guest", "guest@example.com");
		var game = new Game { RawgId = 1, Name = "Test", CoverUrl = "x" };

		_dbContext.Games.Add(game);
		await _dbContext.SaveChangesAsync();

		var ev = new Event
		{
			HostId = host.Id,
			GameId = game.Id,
			Title = "Fika night",
			StartTime = DateTime.UtcNow.AddDays(1),
			EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
			MaxPlayers = 4,
			CreatedAt = DateTime.UtcNow
		};

		_dbContext.UserGames.Add(new UserGame { UserId = host.Id, GameId = game.Id });
		_dbContext.Events.Add(ev);
		await _dbContext.SaveChangesAsync();
		_dbContext.EventParticipants.Add(new EventParticipant { EventId = ev.Id, UserId = guest.Id });
		await _dbContext.SaveChangesAsync();

		var result = await _userService.DeleteUser(host.Id);

		Assert.True(result.Succeeded);
		Assert.False(await _dbContext.UserGames.AnyAsync(ug => ug.UserId == host.Id));
		Assert.False(await _dbContext.Events.AnyAsync(e => e.Id == ev.Id));
		Assert.False(await _dbContext.EventParticipants.AnyAsync(p => p.EventId == ev.Id));
		Assert.True(await _dbContext.Users.AnyAsync(u => u.Id == guest.Id));
		Assert.True(await _dbContext.Games.AnyAsync(g => g.Id == game.Id));
	}
}
