using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IMovieService
{
    Task<MovieDetailsDTO?> GetMovieAsync(int movieId, string language = "en-US");
    Task<List<PopularMovieDTO>> GetPopularMoviesAsync();
    Task<List<MovieDTO>> SearchMovie(string searchQuery, string language = "en-US");
    Task<BulkImportResultDTO> BulkImportFromTmdbAsync(List<int> tmdbIds);
    Task<BulkImportResultDTO> BulkImportFromPagesAsync();
}
