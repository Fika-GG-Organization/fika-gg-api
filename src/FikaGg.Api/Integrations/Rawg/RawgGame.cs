using System.Text.Json.Serialization;

namespace FikaGg.Api.Integrations.Rawg;

// RAWG's json uses snake_case, whilst c# prefers pascalcase, so when we fetch it we get background_image.
// For this reason we have to call on json serilization to interpet background_image as BackgroundImage
public record RawgGame(int Id, string Name, [property: JsonPropertyName("background_image")] string? BackgroundImage);
