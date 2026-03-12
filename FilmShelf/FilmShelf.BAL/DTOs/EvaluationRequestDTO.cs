namespace FilmShelf.BAL.DTOs;

public class EvaluationRequestDTO
{
    public int K { get; set; }
    public int MinReviews { get; set; }
    public byte RelevanceThreshold { get; set; }
}
