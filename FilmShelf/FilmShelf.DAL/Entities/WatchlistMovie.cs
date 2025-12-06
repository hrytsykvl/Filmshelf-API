namespace FilmShelf.DAL.Entities;

public class WatchlistMovie
{
    public int WatchlistId { get; set; }
    public int MovieId { get; set; }
    public DateTime AddedAt { get; set; }

    public UserWatchlist Watchlist { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
