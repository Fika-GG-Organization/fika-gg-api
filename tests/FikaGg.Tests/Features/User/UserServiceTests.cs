using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.User;
using FikaGg.Api.Features.User.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using static FikaGg.Tests.GetGameDetailsAsyncTests;

namespace FikaGg.Tests.Features.User;

public class UserServiceTests
{
	private const string TestPassword = "TestPassword123!"; // same password CreateUserAsync uses

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
			.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
			.Options;

		_dbContext = new AppDbContext(options);

		var store = new UserStore<ApplicationUser>(_dbContext);

		//Custom token provider to enable token handling by custom userManager.
		//Allows the ChangeEmailAsync method to operate as designed in tests.
		var identityOptions = new IdentityOptions();
		identityOptions.User.RequireUniqueEmail = true;
		
		_userManager = new UserManager<ApplicationUser>(
			store,
			Options.Create(identityOptions),
			new PasswordHasher<ApplicationUser>(),
			new[] { new UserValidator<ApplicationUser>() },
			new[] { new PasswordValidator<ApplicationUser>() },
			new UpperInvariantLookupNormalizer(),
			new IdentityErrorDescriber(),
			null!,
			NullLogger<UserManager<ApplicationUser>>.Instance);

		_userService = new UserService(_dbContext, _userManager, new FakeRawgClient());

		_userManager.RegisterTokenProvider(
			TokenOptions.DefaultProvider,
			new DataProtectorTokenProvider<ApplicationUser>(
			new EphemeralDataProtectionProvider(),
			Options.Create(new DataProtectionTokenProviderOptions()),
			NullLogger<DataProtectorTokenProvider<ApplicationUser>>.Instance));
	}


	// HELPERS:

	// Method to create users.
	private async Task<ApplicationUser> CreateUserAsync(
		string? displayName = null,
		string? email = null)
	{
		email ??= "testuser1@example.com";
		displayName ??= "Test User";

		var user = new ApplicationUser
		{
			DisplayName = displayName,
			Email = email,
			UserName = email
		};
		var result = await _userManager.CreateAsync(user, TestPassword);
		Assert.True(result.Succeeded);
		return user;
	}

	// Method that clears the change tracker so assertions read persisted data.
	private async Task<ApplicationUser> ReloadUserAsync(string id)
	{
		_dbContext.ChangeTracker.Clear();
		return await _dbContext.Users.SingleAsync(u => u.Id == id);
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
		Assert.Contains(result.Errors, e => e.Code == "NoUser");
	}

	[Fact]
	public async Task DeleteUser_RemovesTheirGamesParticipationsAndHostedEvents()
	{
		// Arrange
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

		// Act
		var result = await _userService.DeleteUser(host.Id);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(await _dbContext.UserGames.AnyAsync(ug => ug.UserId == host.Id));
		Assert.False(await _dbContext.Events.AnyAsync(e => e.Id == ev.Id));
		Assert.False(await _dbContext.EventParticipants.AnyAsync(p => p.EventId == ev.Id));
		Assert.True(await _dbContext.Users.AnyAsync(u => u.Id == guest.Id));
		Assert.True(await _dbContext.Games.AnyAsync(g => g.Id == game.Id));
	}

	// Fadi: checks if the gamertag gets saved and trimmed
	[Fact]
	public async Task UpdateGamerTagAsync_OwnedGame_SetsTrimmedTag()
	{
		_dbContext.UserGames.Add(new UserGame { UserId = "u1", GameId = 1 });
		await _dbContext.SaveChangesAsync();

		await _userService.UpdateGamerTagAsync("u1", 1, " alexk#EUW ");

		var userGame = await _dbContext.UserGames.SingleAsync();
		Assert.Equal("alexk#EUW", userGame.GamerTag);
	}

	// Fadi: checks if an empty gamer tag sets the value to null.
	[Fact]
	public async Task UpdateGamerTagAsync_EmptyTag_SetsNull()
	{
		_dbContext.UserGames.Add(new UserGame { UserId = "u1", GameId = 1, GamerTag = "alexk#EUW" });
		await _dbContext.SaveChangesAsync();

		await _userService.UpdateGamerTagAsync("u1", 1, "");

		Assert.Null((await _dbContext.UserGames.SingleAsync()).GamerTag);
	}

	// Fadi: checks if removing a game only removes it from the users games
	[Fact]
	public async Task RemoveGameFromUserAsync_OwnedGame_RemovesIt()
	{
		_dbContext.UserGames.Add(new UserGame { UserId = "u1", GameId = 1 });
		await _dbContext.SaveChangesAsync();

		await _userService.RemoveGameFromUserAsync("u1", 1);

		Assert.Empty(_dbContext.UserGames);
	}

	// Fadi: checks if removing someone elses game throws instead of removing it
	[Fact]
	public async Task RemoveGameFromUserAsync_SomeoneElsesGame_Throws()
	{
		_dbContext.UserGames.Add(new UserGame { UserId = "u2", GameId = 1 });
		await _dbContext.SaveChangesAsync();

		await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.RemoveGameFromUserAsync("u1", 1));
	}

	// ---------- UpdateUserProfileAsync ----------

	[Fact]
	public async Task UpdateUserProfileAsync_NullDto_ReturnsEmptyObjectError()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(null!, user.Id);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "EmptyObject");
	}

	[Fact]
	public async Task UpdateUserProfileAsync_UnknownUser_ReturnsNoUserError()
	{
		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { DisplayName = "Someone" }, "no-such-id");

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "NoUser");
	}

	[Fact]
	public async Task UpdateUserProfileAsync_EmptyDto_SucceedsAndChangesNothing()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(new UpdateProfileDTO(), user.Id);

		// Assert
		Assert.True(result.Succeeded);
		var reloaded = await ReloadUserAsync(user.Id);
		Assert.Equal("Test User", reloaded.DisplayName);
		Assert.Equal("testuser1@example.com", reloaded.Email);
		Assert.True(await _userManager.CheckPasswordAsync(reloaded, TestPassword));
	}

	// -- display name --

	[Fact]
	public async Task UpdateUserProfileAsync_DisplayName_IsUpdatedWithoutPassword()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { DisplayName = "Alexander" }, user.Id);

		// Assert
		Assert.True(result.Succeeded);
		Assert.Equal("Alexander", (await ReloadUserAsync(user.Id)).DisplayName);
	}

	[Fact]
	public async Task UpdateUserProfileAsync_DisplayName_IsTrimmed()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		await _userService.UpdateUserProfileAsync(new UpdateProfileDTO { DisplayName = "  Alex  " }, user.Id);

		// Assert
		Assert.Equal("Alex", (await ReloadUserAsync(user.Id)).DisplayName);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public async Task UpdateUserProfileAsync_BlankDisplayName_IsIgnored(string? displayName)
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { DisplayName = displayName }, user.Id);

		// Assert
		Assert.True(result.Succeeded);
		Assert.Equal("Test User", (await ReloadUserAsync(user.Id)).DisplayName);
	}

	// -- email --

	[Fact]
	public async Task UpdateUserProfileAsync_Email_WithCorrectPassword_UpdatesEmailAndUserName()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { Email = "new@example.com", CurrentPassword = TestPassword }, user.Id);

		// Assert
		Assert.True(result.Succeeded);
		var reloaded = await ReloadUserAsync(user.Id);
		Assert.Equal("new@example.com", reloaded.Email);
		// login goes through UserName, so it must follow the email
		Assert.Equal("new@example.com", reloaded.UserName);
		Assert.NotNull(await _userManager.FindByNameAsync("new@example.com"));
		Assert.Null(await _userManager.FindByNameAsync("testuser1@example.com"));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("WrongPassword1!")]
	public async Task UpdateUserProfileAsync_Email_WithMissingOrWrongPassword_IsRejected(string? currentPassword)
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { Email = "new@example.com", CurrentPassword = currentPassword }, user.Id);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "CredentialMismatch");
		var reloaded = await ReloadUserAsync(user.Id);
		Assert.Equal("testuser1@example.com", reloaded.Email);
		Assert.Equal("testuser1@example.com", reloaded.UserName);
	}

	[Fact]
	public async Task UpdateUserProfileAsync_Email_SameAddressDifferentCasing_IsNoOpAndNeedsNoPassword()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { Email = "TESTUSER1@EXAMPLE.COM" }, user.Id);

		// Assert
		Assert.True(result.Succeeded);
		Assert.Equal("testuser1@example.com", (await ReloadUserAsync(user.Id)).Email);
	}

	[Fact]
	public async Task UpdateUserProfileAsync_Email_UsedByAnotherUser_IsRejected()
	{
		// Arrange
		var user = await CreateUserAsync();
		await CreateUserAsync("Other", "taken@example.com");

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { Email = "taken@example.com", CurrentPassword = TestPassword }, user.Id);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "DuplicateEmail");
		Assert.Equal("testuser1@example.com", (await ReloadUserAsync(user.Id)).Email);
	}

	// -- password --

	[Fact]
	public async Task UpdateUserProfileAsync_Password_WithCorrectCurrentPassword_IsChanged()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { CurrentPassword = TestPassword, NewPassword = "NewPassword456!" }, user.Id);

		// Assert
		Assert.True(result.Succeeded);
		var reloaded = await ReloadUserAsync(user.Id);
		Assert.True(await _userManager.CheckPasswordAsync(reloaded, "NewPassword456!"));
		Assert.False(await _userManager.CheckPasswordAsync(reloaded, TestPassword));
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("WrongPassword1!")]
	public async Task UpdateUserProfileAsync_Password_WithMissingOrWrongCurrentPassword_IsRejected(string? currentPassword)
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { CurrentPassword = currentPassword, NewPassword = "NewPassword456!" }, user.Id);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "CredentialMismatch");
		Assert.True(await _userManager.CheckPasswordAsync(await ReloadUserAsync(user.Id), TestPassword));
	}

	[Fact]
	public async Task UpdateUserProfileAsync_Password_FailingValidation_IsRejected()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(
			new UpdateProfileDTO { CurrentPassword = TestPassword, NewPassword = "alllowercase" }, user.Id);

		// Assert
		Assert.False(result.Succeeded);
		Assert.True(await _userManager.CheckPasswordAsync(await ReloadUserAsync(user.Id), TestPassword));
	}

	// -- combined --

	[Fact]
	public async Task UpdateUserProfileAsync_AllThreeFields_AreUpdatedTogether()
	{
		// Arrange
		var user = await CreateUserAsync();

		// Act
		var result = await _userService.UpdateUserProfileAsync(new UpdateProfileDTO
		{
			DisplayName = "Alexander",
			Email = "new@example.com",
			CurrentPassword = TestPassword,
			NewPassword = "NewPassword456!"
		}, user.Id);

		// Assert
		Assert.True(result.Succeeded);
		var reloaded = await ReloadUserAsync(user.Id);
		Assert.Equal("Alexander", reloaded.DisplayName);
		Assert.Equal("new@example.com", reloaded.Email);
		Assert.Equal("new@example.com", reloaded.UserName);
		Assert.True(await _userManager.CheckPasswordAsync(reloaded, "NewPassword456!"));
	}

	[Fact]
	public async Task UpdateUserProfile_DuplicateDisplayName_IsRejected()
	{
		// Arrange
		var takenUser = await CreateUserAsync("TakenDisplayName", "taken@test.com");
		var updatingUser = await CreateUserAsync("NewDisplayName", "newdn@test.com");

		// Act
		var result = await _userService.UpdateUserProfileAsync(new UpdateProfileDTO
		{ DisplayName= "TakenDisplayName" }, updatingUser.Id);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains(result.Errors, e => e.Code == "DuplicateDisplayName");
	}
}
