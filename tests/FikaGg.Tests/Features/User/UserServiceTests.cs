using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.User;
using FikaGg.Api.Features.User.Interfaces;
using FikaGg.Api.Integrations.Rawg;
using FikaGg.Tests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
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
		// Arrange
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

	[Fact]
	public async Task DeleteUser_Using_UserId_Should_Succeed()
	{
		// Arrange
		var user = new ApplicationUser
		{
			DisplayName = "Test User",
			Email = "existing@example.com",
			UserName = "existing@example.com"
		};
		var create = await _userManager.CreateAsync(user, "TestPassword123!");
		Assert.True(create.Succeeded);

		// Act
		var result = await _userService.DeleteUser(user.Id);

		// Assert
		Assert.True(result.Succeeded);
		Assert.False(await _dbContext.Users.AnyAsync(u => u.Id == user.Id));
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
}
