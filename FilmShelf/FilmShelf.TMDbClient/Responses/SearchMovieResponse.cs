using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class SearchMovieResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = null!;

    [JsonPropertyName("release_date")]
    public DateTime? ReleaseDate { get; set; }

    [JsonPropertyName("vote_average")]
    public float AverageRating { get; set; }
}
