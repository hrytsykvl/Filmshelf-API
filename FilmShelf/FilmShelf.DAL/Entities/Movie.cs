namespace FilmShelf.DAL.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int GenreId { get; set; }
    public int DirectorId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int Runtime { get; set; }
    public float AverageRating { get; set; }

    public Genre Genre { get; set; } = null!;
    public Director Director { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new List<Review>();
    public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
