using FikaGg.Api.Features.Notices.DTOs;

namespace FikaGg.Api.Features.Notices.Interfaces;

public interface INoticeService
{
	Task<List<NoticeDTO>> GetNoticesAsync(string userId, bool unreadOnly = false);
	Task<int> GetUnreadCountAsync(string userId);
	Task MarkAsViewedAsync(int noticeId, string userId);
	Task<int> MarkAllAsViewedAsync(string userId);
	Task DeleteNoticeAsync(int noticeId, string userId);

	// This exists to be called by other services and is not to be exposed through the controller.
	Task CreateNoticeAsync(string userId, string title, string message);
}
