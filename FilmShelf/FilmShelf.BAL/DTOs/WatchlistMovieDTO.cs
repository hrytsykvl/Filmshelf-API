namespace FilmShelf.BAL.DTOs;

public class WatchlistMovieDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public float AverageRating { get; set; }
}
