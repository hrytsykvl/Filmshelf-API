namespace FilmShelf.API.VMs;

public class WatchlistCheckVM
{
    public int WatchlistId { get; set; }
    public List<int> MovieIds { get; set; } = new();
}
