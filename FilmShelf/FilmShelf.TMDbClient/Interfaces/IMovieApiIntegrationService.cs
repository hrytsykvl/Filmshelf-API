using FilmShelf.TMDbClient.Responses;

namespace FilmShelf.TMDbClient.Interfaces;

public interface IMovieApiIntegrationService
{
    Task<MovieDetailsResponse?> FetchMovieDetailsAsync(int movieId, string language = "en-US");
    Task<MovieCreditsResponse?> FetchMovieCreditsAsync(int movieId, string language = "en-US");
    Task<List<SearchMovieResponse>> SearchMovie(string searchQuery, string language = "en-US");
    Task<(string? JsonResponse, List<PopularMovieResponse> Movies)> FetchPopularMoviesAsync(int count, string language = "en-US");
    Task<T?> FetchPersonDetailsAsync<T>(int personId, string language = "en-US");
    Task<string?> FetchMoviesPageAsync(int pageNumber, string language = "en-US");
}
