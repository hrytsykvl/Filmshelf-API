namespace FilmShelf.API.VMs;

public class LlmRecommendationVM
{
    public MovieResponseVM Movie { get; set; } = null!;
    public float Score { get; set; }
    public string Reason { get; set; } = null!;
}
