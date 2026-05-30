namespace FilmShelf.DAL.Entities;

public class LlmRecommendationCache
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Provider { get; set; } = null!;
    public string ResponseJson { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
