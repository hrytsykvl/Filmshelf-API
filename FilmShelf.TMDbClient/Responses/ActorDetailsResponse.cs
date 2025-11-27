using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class ActorDetailsResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("birthday")]
    public DateTime Birthday { get; set; }

    [JsonPropertyName("profile_path")]
    public string ProfilePath { get; set; } = null!;

    [JsonPropertyName("biography")]
    public string Biography { get; set; } = null!;
}
