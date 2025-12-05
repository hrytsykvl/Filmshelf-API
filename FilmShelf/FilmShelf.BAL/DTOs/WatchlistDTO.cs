namespace FilmShelf.BAL.DTOs;

public class WatchlistDTO
{
    public List<WatchlistMovieDTO> Movies { get; set; } = new List<WatchlistMovieDTO>();
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int TotalMovies { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
