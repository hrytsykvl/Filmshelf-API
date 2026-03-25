namespace FilmShelf.API.VMs;

public class LlmComparisonResultVM
{
    public List<LlmRecommendationVM> ClaudeRecommendations { get; set; } = new();
    public List<LlmRecommendationVM> LlamaRecommendations { get; set; } = new();
    public double ClaudeResponseTimeMs { get; set; }
    public double LlamaResponseTimeMs { get; set; }
    public DateTime GeneratedAt { get; set; }
}
