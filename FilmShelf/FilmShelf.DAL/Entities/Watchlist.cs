using FilmShelf.DAL.Identity;

namespace FilmShelf.DAL.Entities;

public class Watchlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public DateTime AddedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
