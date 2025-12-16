namespace FilmShelf.API.VMs;

public class WatchlistCheckVM
{
    public int WatchlistId { get; set; }
    public string Title { get; set; } = null!;
    public List<int> MovieIds { get; set; } = new();
}
