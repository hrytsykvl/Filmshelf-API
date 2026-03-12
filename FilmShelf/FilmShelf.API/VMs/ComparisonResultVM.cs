namespace FilmShelf.API.VMs;

public class ComparisonResultVM
{
    public List<MovieResponseVM> MlRecommendations { get; set; } = new();
    public List<LlmRecommendationVM> LlmRecommendations { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
