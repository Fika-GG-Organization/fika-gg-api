using FikaGg.Api.Data;
using FikaGg.Api.Entities;
using FikaGg.Api.Features.Notices.DTOs;
using FikaGg.Api.Features.Notices.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace FikaGg.Api.Features.Notices.Service;

public class NoticeService(AppDbContext context) : INoticeService
{
	/// <summary>
	/// Retrieves a list notices for a user 
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="unreadOnly"></param>
	/// <returns></returns>
	public async Task<List<NoticeDTO>> GetNoticesAsync(string userId, bool unreadOnly = false)
	{
		var query = context.Notices.Where(n => n.UserId == userId);

		if (unreadOnly)
			query = query.Where(n => !n.Viewed);

		return await query
			.OrderByDescending(n => n.CreatedAt)
			.Select(n => new NoticeDTO(
				n.Id,
				n.Title,
				n.Message,
				n.Viewed,
				n.CreatedAt))
			.ToListAsync();

	}

	/// <summary>
	/// Retrieves the number of unread notices that a user has
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	public async Task<int> GetUnreadCountAsync(string userId)
	{
		var result = await context.Notices.CountAsync(n => n.UserId == userId && !n.Viewed);
		return result;
	}

	/// <summary>
	/// Marks notices as viewed
	/// </summary>
	/// <param name="noticeId"></param>
	/// <param name="userId"></param>
	/// <returns></returns>
	/// <exception cref="KeyNotFoundException"></exception>
	public async Task MarkAsViewedAsync(int noticeId, string userId)
	{
		var notice = await context.Notices
			.FirstOrDefaultAsync(n => n.Id == noticeId && n.UserId == userId)
			?? throw new KeyNotFoundException("Notice not found");
		notice.Viewed = true;
		await context.SaveChangesAsync();
	}

	/// <summary>
	/// Marks all notices as viewed
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>
	public async Task<int> MarkAllAsViewedAsync(string userId)
	{
		var result = await context.Notices
			.Where(n => n.UserId == userId && !n.Viewed)
			.ExecuteUpdateAsync(s => s.SetProperty(n => n.Viewed, true));
		return result;
	}

	/// <summary>
	/// Creates a new notice for a user	
	/// </summary>
	/// <param name="userId"></param>
	/// <param name="title"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public async Task CreateNoticeAsync(string userId, string title, string message)
	{
		context.Notices.Add(new Notice
		{
			UserId = userId,
			Title = title,
			Message = message
		});

		await context.SaveChangesAsync();
	}

	/// <summary>
	/// Deletes a notice for a user
	/// </summary>
	/// <param name="noticeId"></param>
	/// <param name="userId"></param>
	/// <returns></returns>
	/// <exception cref="KeyNotFoundException"></exception>
	public async Task DeleteNoticeAsync(int noticeId, string userId)
	{
		var deleted = await context.Notices
			.Where(n => n.Id == noticeId && n.UserId == userId)
			.ExecuteDeleteAsync();

		if (deleted == 0)
			throw new KeyNotFoundException("Notice not found");
	}
}
