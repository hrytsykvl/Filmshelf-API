namespace FilmShelf.API.VMs;

public class EvaluationRequestVM
{
    public int K { get; set; } = 10;
    public int MinReviews { get; set; } = 5;
    public byte RelevanceThreshold { get; set; } = 7;
}
