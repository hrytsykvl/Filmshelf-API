namespace FilmShelf.BAL.DTOs;

public class EvaluationRequestDTO
{
    public int K { get; set; }
    public int MinReviews { get; set; }
    public byte RelevanceThreshold { get; set; }
    /// <summary>
    /// Maximum number of users to evaluate for expensive methods (llm, embedding).
    /// Null means no cap.
    /// </summary>
    public int? LlmMaxUsers { get; set; }
}
