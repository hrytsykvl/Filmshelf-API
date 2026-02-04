using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class MovieCreditsResponse
{
    [JsonPropertyName("cast")]
    public List<CastMemberResponse> Cast { get; set; } = new();

    [JsonPropertyName("crew")]
    public List<CrewMemberResponse> Crew { get; set; } = new();
}
