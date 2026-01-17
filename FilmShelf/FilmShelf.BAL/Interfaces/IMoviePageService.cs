using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IMoviePageService
{
    Task FetchAndUpdateAsync();
    Task<(IEnumerable<MovieDTO> Movies, int totalPages)> GetMoviesOnPageAsync(int pageNumber);
    Task<IEnumerable<MovieDTO>> GetPopularMoviesPageAsync();
}
