using FilmShelf.TMDbClient.Responses;

namespace FilmShelf.TMDbClient.Interfaces;

public interface IMovieApiIntegrationService
{
    Task<MovieDetailsResponse?> FetchMovieDetailsAsync(int movieId);
    Task<MovieCreditsResponse?> FetchMovieCreditsAsync(int movieId);
    Task<DirectorDetailsResponse?> FetchDirectorDetailsAsync(int directorId);
    Task<string?> FetchMoviesPageAsync(int pageNumber);
}
