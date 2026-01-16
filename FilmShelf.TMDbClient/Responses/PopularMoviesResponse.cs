using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class PopularMoviesResponse
{
    [JsonPropertyName("results")]
    public List<PopularMovieResponse> Results { get; set; } = new();
}
