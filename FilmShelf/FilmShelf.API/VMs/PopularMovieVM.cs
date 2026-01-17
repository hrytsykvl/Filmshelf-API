namespace FilmShelf.API.VMs;

public class PopularMovieVM
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public float AverageRating { get; set; }
    public DateTime ReleaseDate { get; set; }
}
