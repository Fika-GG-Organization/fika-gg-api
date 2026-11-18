using System.ComponentModel.DataAnnotations;

namespace FikaGg.Api.Features.User;

// null or empty removes the gamertag
public record UpdateGamerTagDTO([MaxLength(50)] string? GamerTag);
