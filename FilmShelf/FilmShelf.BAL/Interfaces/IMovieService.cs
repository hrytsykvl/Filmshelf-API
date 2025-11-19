using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IMovieService
{
    Task<MovieDetailsDTO?> GetMovieAsync(int movieId);
}
