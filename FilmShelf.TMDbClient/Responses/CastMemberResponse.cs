using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class CastMemberResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("character")]
    public string Character { get; set; } = null!;

    [JsonPropertyName("profile_path")]
    public string ProfilePath { get; set; } = null!;
}
