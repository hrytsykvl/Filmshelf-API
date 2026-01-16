namespace FilmShelf.BAL.DTOs;

public class PopularMovieDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public float AverageRating { get; set; }
    public DateTime ReleaseDate { get; set; }
}
