using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Avatars;
using FikaGg.Api.Integrations.CloudinaryConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace FikaGg.Tests.Features.Avatars;

public class AvatarServiceTests
{
	private readonly AppDbContext _db;
	private readonly IImageStorageService _storage = Substitute.For<IImageStorageService>();
	private readonly AvatarService _avatarService;

	private static CancellationToken Ct => TestContext.Current.CancellationToken;

	public AvatarServiceTests()
	{
		var options = new DbContextOptionsBuilder<AppDbContext>()
			// Use a unique in-memory database for each test to ensure isolation
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		_db = new AppDbContext(options);

		// Make the fake storage return the same result for every upload
		_storage.UploadAvatarAsync(default!, default!, default!, default)
			.ReturnsForAnyArgs(new UploadedImage("new-url", "new-id"));

		_avatarService = new AvatarService(_db, _storage);
	}

	// Helper method to add a user to the in-memory database
	private async Task<ApplicationUser> AddUser(string? avatarPublicId = null)
	{
		var user = new ApplicationUser
		{
			Id = "user-1",
			UserName = "testuser",
			AvatarUrl = avatarPublicId is null ? null : "old-url",
			AvatarPublicId = avatarPublicId
		};
		await _db.Users.AddAsync(user, Ct);
		await _db.SaveChangesAsync(Ct);
		return user;
	}

	// Helper method to create a fake IFormFile for testing
	private static FormFile FakeFile() => new(new MemoryStream([1, 2, 3]), 0, 3, "file", "avatar.png");

	[Fact]
	public async Task UploadAsync_ReturnsNull_WhenUserDoesNotExist()
	{
		var result = await _avatarService.UploadAsync("missing-user", FakeFile(), Ct);

		Assert.Null(result);
	}

	[Fact]
	public async Task UploadAsync_SavesNewAvatar_AndDeletesOldOne()
	{
		var user = await AddUser(avatarPublicId: "old-id");

		var result = await _avatarService.UploadAsync(user.Id, FakeFile(), Ct);

		Assert.Equal("new-url", result);
		Assert.Equal("new-id", user.AvatarPublicId);
		await _storage.Received().DeleteAsync("old-id", Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task DeleteAsync_ClearsAvatar_AndDeletesFromStorage()
	{
		var user = await AddUser(avatarPublicId: "some-id");

		var result = await _avatarService.DeleteAsync(user.Id, Ct);

		Assert.True(result);
		Assert.Null(user.AvatarPublicId);
		await _storage.Received().DeleteAsync("some-id", Arg.Any<CancellationToken>());
	}
}
