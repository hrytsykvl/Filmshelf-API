using System.ComponentModel.DataAnnotations;

namespace FilmShelf.DAL.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Overview { get; set; } = null!;
    public int DirectorId { get; set; }
    public DateTime ReleaseDate { get; set; }
    public short Runtime { get; set; }
    public string PosterPath { get; set; } = null!;

    [Range(0, 10)]
    public float AverageRating { get; set; }

    public Director Director { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new List<Review>();
    public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
