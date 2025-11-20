using System.Text.Json.Serialization;

namespace FilmShelf.BAL.DTOs;

public class MovieDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("poster_path")]
    public string PosterPath { get; set; } = null!;
}
