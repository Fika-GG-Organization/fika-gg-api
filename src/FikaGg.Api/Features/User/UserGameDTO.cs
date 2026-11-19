namespace FikaGg.Api.Features.User;

// a game the user has on their profile, GameId is the id the entity represents in the database.
public record UserGameDTO(int GameId, int? RawgId, string Name, string? CoverUrl, string? GamerTag);
