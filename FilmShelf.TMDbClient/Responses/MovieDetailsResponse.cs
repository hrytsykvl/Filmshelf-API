using System.Text.Json.Serialization;

namespace FilmShelf.TMDbClient.Responses;

public class MovieDetailsResponse
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("genres")]
    public List<GenreResponse> Genres { get; set; } = new();

    [JsonPropertyName("overview")]
    public string Overview { get; set; } = null!;

    [JsonPropertyName("release_date")]
    public DateTime ReleaseDate { get; set; }

    [JsonPropertyName("runtime")]
    public short Runtime { get; set; }

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = null!;

    [JsonPropertyName("vote_average")]
    public float AverageRating { get; set; }
}
