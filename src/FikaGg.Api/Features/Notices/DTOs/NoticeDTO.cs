namespace FikaGg.Api.Features.Notices.DTOs;

public record NoticeDTO(int Id, string Title, string Message, bool Viewed, DateTime CreatedAt);
