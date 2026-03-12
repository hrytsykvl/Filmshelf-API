namespace FilmShelf.API.VMs;

public class BulkImportResultVM
{
    public int Saved { get; set; }
    public int Skipped { get; set; }
    public int Failed { get; set; }
    public List<int> FailedIds { get; set; } = new();
}
