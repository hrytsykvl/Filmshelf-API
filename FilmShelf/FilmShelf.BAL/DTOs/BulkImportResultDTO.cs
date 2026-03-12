namespace FilmShelf.BAL.DTOs;

public class BulkImportResultDTO
{
    public int Saved { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public List<int> FailedIds { get; set; } = new();
}
