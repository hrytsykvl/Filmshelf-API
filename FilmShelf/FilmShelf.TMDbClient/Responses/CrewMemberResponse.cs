using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class CrewMemberResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("job")]
    public string Job { get; set; } = null!;
}
