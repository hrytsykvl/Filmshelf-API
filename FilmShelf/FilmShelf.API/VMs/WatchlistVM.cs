namespace FilmShelf.API.VMs;

public class WatchlistVM
{
    public List<WatchlistMovieVM> Movies { get; set; } = new List<WatchlistMovieVM>();
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int TotalMovies { get; set; }
    public DateTime? UpdatedAt { get; set; }
}