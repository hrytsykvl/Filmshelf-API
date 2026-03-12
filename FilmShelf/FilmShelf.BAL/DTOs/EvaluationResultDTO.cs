namespace FilmShelf.BAL.DTOs;

public class EvaluationResultDTO
{
    public string Method { get; set; } = null!;
    public int K { get; set; }
    public int EvaluatedUsers { get; set; }
    public int SkippedUsers { get; set; }
    public double HitRateAtK { get; set; }
    public double PrecisionAtK { get; set; }
    public double RecallAtK { get; set; }
    public double NdcgAtK { get; set; }
    public double Mrr { get; set; }
    public double CatalogCoverage { get; set; }
    public double EvaluationDurationSeconds { get; set; }
    public DateTime EvaluatedAt { get; set; }
}
