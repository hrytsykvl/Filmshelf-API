namespace FilmShelf.BAL.DTOs;

public class WatchlistCheckDTO
{
    public int WatchlistId { get; set; }
    public List<int> MovieIds { get; set; } = new();
}
