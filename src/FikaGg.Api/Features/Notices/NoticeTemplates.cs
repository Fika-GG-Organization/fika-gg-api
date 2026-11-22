using FikaGg.Api.Entities;

namespace FikaGg.Api.Features.Notices;

public static class NoticeTemplates
{
	public static Notice EventDeleted(string userId, string eventTitle) => new Notice
	{
		UserId = userId,
		Title = "Evenemang borttaget",
		Message = $"Evenemanget \"{eventTitle}\" har tagits bort av värden.",
		CreatedAt = DateTime.UtcNow,
	};

	public static Notice HostMigrated(string userId, string eventTitle) => new Notice
	{
		UserId = userId,
		Title = "Du är ny värd!",
		Message = $"Du har blivit värd för evenemanget \"{eventTitle}\".",
		CreatedAt = DateTime.UtcNow,
	};

	public static Notice PromotedFromReserve(string userId, string eventTitle) => new Notice
	{
		UserId = userId,
		Title = "Du har fått en plats!",
		Message = $"En plats blev ledig i \"{eventTitle}\" Du är nmu med i evenemanget",
		CreatedAt = DateTime.UtcNow,
	};
}
