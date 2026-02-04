using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class PopularMovieResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = null!;

    [JsonPropertyName("vote_average")]
    public float AverageRating { get; set; }

    [JsonPropertyName("release_date")]
    public DateTime ReleaseDate { get; set; }
}
