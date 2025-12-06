using FilmShelf.DAL.Identity;

namespace FilmShelf.DAL.Entities;

public class UserWatchlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public bool IsDefault { get; set; } = false;

    public ApplicationUser User { get; set; } = null!;
    public List<WatchlistMovie> WatchlistMovies { get; set; } = new List<WatchlistMovie>();
}
