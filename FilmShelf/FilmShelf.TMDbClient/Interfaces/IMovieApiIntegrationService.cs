using FilmShelf.TMDbClient.Responses;

namespace FilmShelf.TMDbClient.Interfaces;

public interface IMovieApiIntegrationService
{
    Task<MovieDetailsResponse?> FetchMovieDetailsAsync(int movieId);
    Task<MovieCreditsResponse?> FetchMovieCreditsAsync(int movieId);
    Task<(string? JsonResponse, List<PopularMovieResponse> Movies)> FetchPopularMoviesAsync(int count);
    Task<T?> FetchPersonDetailsAsync<T>(int personId);
    Task<string?> FetchMoviesPageAsync(int pageNumber);
}
