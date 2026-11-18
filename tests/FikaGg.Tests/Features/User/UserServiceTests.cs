using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.User;
using FikaGg.Api.Features.User.Interfaces;
using FikaGg.Api.Integrations.Rawg;
using FikaGg.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using static FikaGg.Tests.GetGameDetailsAsyncTests;

namespace FikaGg.Tests.Features.User;
public class UserServiceTests
{
	private readonly AppDbContext _dbContext;
	private readonly UserManager<ApplicationUser> _userManager = Substitute.For<UserManager<ApplicationUser>>(
		Substitute.For<IUserStore<ApplicationUser>>(),
		null, null, null, null, null, null, null, null);
	private readonly UserService _userService;

	public UserServiceTests()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_dbContext = new AppDbContext(options);
		_userService = new UserService(_dbContext, _userManager, new FakeRawgClient());
	}

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
		_userManager
			.CreateAsync(Arg.Any<ApplicationUser>(), dto.Password)
			.Returns(IdentityResult.Success);

		// Act
		var result = await _userService.RegisterUserAsync(dto);

		// Assert
		Assert.True(result.Succeeded);
		await _userManager.Received(1).CreateAsync(
			Arg.Is<ApplicationUser>(u => u.Email == dto.Email && u.DisplayName == dto.DisplayName),
			dto.Password);
	}

	[Fact]
	public async Task RegisterUserAsync_DuplicateDisplayName_ShouldFail()
	{
		// Arrange: seed an existing user with the same display name
		_dbContext.Users.Add(new ApplicationUser
		{
			DisplayName = "Test User",
			Email = "existing@example.com",
			UserName = "existing@example.com"
		});
		await _dbContext.SaveChangesAsync();

		var duplicateDto = Generate_Duplicate_DisplayName();

		// Act
		var result = await _userService.RegisterUserAsync(duplicateDto);

		// Assert
		Assert.False(result.Succeeded);
		await _userManager.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!);
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
}
