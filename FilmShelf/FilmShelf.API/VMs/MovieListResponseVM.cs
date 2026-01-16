namespace FilmShelf.API.VMs;

public class MovieListResponseVM
{
    public List<MovieResponseVM> MovieList { get; set; } = new();
    public int? TotalPages { get; set; }
}
