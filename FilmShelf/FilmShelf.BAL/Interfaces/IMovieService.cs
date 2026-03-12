using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IMovieService
{
    Task<MovieDetailsDTO?> GetMovieAsync(int movieId);
    Task<List<PopularMovieDTO>> GetPopularMoviesAsync();
    Task<List<MovieDTO>> SearchMovie(string searchQuery);
    Task<BulkImportResultDTO> BulkImportFromTmdbAsync(List<int> tmdbIds);
    Task<BulkImportResultDTO> BulkImportFromPagesAsync();
}
