using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class GenreResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}
