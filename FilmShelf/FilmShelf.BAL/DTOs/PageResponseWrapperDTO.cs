using System.Text.Json.Serialization;

namespace FilmShelf.BAL.DTOs;

public class PageResponseWrapperDTO
{
    [JsonPropertyName("results")]
    public List<MovieDTO> Results { get; set; } = new();


    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
}
