namespace FilmShelf.API.VMs;

public class MovieNotificationVM
{
    public int Id { get; set; }
    public List<PopularMovieVM> PopularMovies { get; set; } = new();
}
