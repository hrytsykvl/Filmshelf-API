using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class DirectorDetailsResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("biography")]
    public string Biography { get; set; } = null!;

    [JsonPropertyName("birthday")]
    public DateTime Birthday { get; set; }
}
