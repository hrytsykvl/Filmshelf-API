using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;
public class SearchMoviesResponse
{
    [JsonPropertyName("results")]
    public List<SearchMovieResponse> Results { get; set; } = new();
}
