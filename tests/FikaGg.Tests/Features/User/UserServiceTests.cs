using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.User;
using FikaGg.Api.Features.User.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

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
		_userService = new UserService(_dbContext, _userManager);
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
}
