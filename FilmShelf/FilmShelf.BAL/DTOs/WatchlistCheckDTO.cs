namespace FilmShelf.BAL.DTOs;

public class WatchlistCheckDTO
{
    public int WatchlistId { get; set; }
    public string Title { get; set; } = null!;
    public List<int> MovieIds { get; set; } = new();
}
