namespace FilmShelf.API.VMs;

public class BulkImportRequestVM
{
    public List<int> TmdbIds { get; set; } = new();
}
