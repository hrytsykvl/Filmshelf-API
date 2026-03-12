namespace FilmShelf.BAL.DTOs;

public class LlmRecommendationDTO
{
    public MovieDTO Movie { get; set; } = null!;
    public float Score { get; set; }
    public string Reason { get; set; } = null!;
}
