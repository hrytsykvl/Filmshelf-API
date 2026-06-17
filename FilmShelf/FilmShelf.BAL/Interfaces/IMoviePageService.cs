using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IMoviePageService
{
    Task FetchAndUpdateAsync();
    Task<(IEnumerable<MovieDTO> Movies, int totalPages)> GetMoviesOnPageAsync(int pageNumber, string language = "en-US");
    Task<IEnumerable<MovieDTO>> GetPopularMoviesPageAsync(string language = "en-US");
}
