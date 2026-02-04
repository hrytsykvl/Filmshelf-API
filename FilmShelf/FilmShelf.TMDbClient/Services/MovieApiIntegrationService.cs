using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.Options;
using FilmShelf.TMDbClient.Responses;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Text.Json;

namespace FilmShelf.TMDbClient.Services;

internal class MovieApiIntegrationService : IMovieApiIntegrationService
{
    private readonly RestClient _restClient;
    private readonly TmdbSettings _tmdbSettings;

    public MovieApiIntegrationService(
        IOptions<TmdbSettings> tmdbSettings,
        RestClient restClient)
    {
        _tmdbSettings = tmdbSettings.Value;
        _restClient = restClient;
    }

    public async Task<T?> FetchPersonDetailsAsync<T>(int personId)
    {
        var request = CreateRequest($"person/{personId}");
        return await _restClient.GetAsync<T>(request);
    }

    public async Task<MovieCreditsResponse?> FetchMovieCreditsAsync(int movieId)
    {
        var request = CreateRequest($"movie/{movieId}/credits");
        return await _restClient.GetAsync<MovieCreditsResponse>(request);
    }

    public async Task<MovieDetailsResponse?> FetchMovieDetailsAsync(int movieId)
    {
        var request = CreateRequest($"movie/{movieId}");
        return await _restClient.GetAsync<MovieDetailsResponse>(request);
    }

    public async Task<(string? JsonResponse, List<PopularMovieResponse> Movies)> FetchPopularMoviesAsync(int count)
    {
        var request = CreateRequest("movie/popular");
        var response = await _restClient.GetAsync(request);

        if (response == null 
            || response.Content == null)
        {
            return (null, new ());
        }

        var popularMoviesResponse = JsonSerializer
            .Deserialize<PopularMoviesResponse>(response.Content);

        var movies = popularMoviesResponse?.Results
            .Take(count)
            .ToList() ?? new ();

        return (response.Content, movies);
    }

    public async Task<string?> FetchMoviesPageAsync(int pageNumber)
    {
        var request = CreateRequest("discover/movie")
            .AddQueryParameter("page", pageNumber.ToString())
            .AddQueryParameter("sort_by", _tmdbSettings.SortBy);

        var response = await _restClient.GetAsync(request);
        return response.Content;
    }

    private RestRequest CreateRequest(string resource)
    {
        return new RestRequest(resource)
            .AddHeader("accept", "application/json")
            .AddHeader("Authorization", $"Bearer {_tmdbSettings.ApiKey}");
    }
}
