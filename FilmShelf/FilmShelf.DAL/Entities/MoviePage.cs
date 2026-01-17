using FilmShelf.DAL.Enums;

namespace FilmShelf.DAL.Entities;

public class MoviePage
{
    public int Id { get; set; }
    public int PageNumber { get; set; }
    public string MoviesJson { get; set; } = null!;
    public DateTime UpdatedAt { get; set; }
    public MoviePageType Type { get; set; } = MoviePageType.Regular;
}
