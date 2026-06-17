using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.JsonConverters;
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

    public async Task<T?> FetchPersonDetailsAsync<T>(int personId, string language = "en-US")
    {
        var request = CreateRequest($"person/{personId}")
            .AddQueryParameter("language", language);
        return await _restClient.GetAsync<T>(request);
    }

    public async Task<MovieCreditsResponse?> FetchMovieCreditsAsync(int movieId, string language = "en-US")
    {
        var request = CreateRequest($"movie/{movieId}/credits")
            .AddQueryParameter("language", language);
        return await _restClient.GetAsync<MovieCreditsResponse>(request);
    }

    public async Task<MovieDetailsResponse?> FetchMovieDetailsAsync(int movieId, string language = "en-US")
    {
        var request = CreateRequest($"movie/{movieId}")
            .AddQueryParameter("language", language);
        return await _restClient.GetAsync<MovieDetailsResponse>(request);
    }

    public async Task<(string? JsonResponse, List<PopularMovieResponse> Movies)> FetchPopularMoviesAsync(int count, string language = "en-US")
    {
        var request = CreateRequest("movie/popular")
            .AddQueryParameter("language", language);
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

    public async Task<string?> FetchMoviesPageAsync(int pageNumber, string language = "en-US")
    {
        var request = CreateRequest("discover/movie")
            .AddQueryParameter("page", pageNumber.ToString())
            .AddQueryParameter("sort_by", _tmdbSettings.SortBy)
            .AddQueryParameter("language", language);

        var response = await _restClient.GetAsync(request);
        return response.Content;
    }

    public async Task<List<SearchMovieResponse>> SearchMovie(string searchQuery, string language = "en-US")
    {
        const int MaxResults = 8;
        var request = CreateRequest("search/movie")
            .AddQueryParameter("query", searchQuery)
            .AddQueryParameter("language", language);

        var response = await _restClient.GetAsync(request);

        if (response == null
            || response.Content == null)
        {
            return new ();
        }

        var options = new JsonSerializerOptions
        {
            Converters = { new NullableDateTimeConverter() },
            PropertyNameCaseInsensitive = true
        };

        var searchResults = JsonSerializer
            .Deserialize<SearchMoviesResponse>(response.Content, options);

        var movies = searchResults?.Results
            .Take(MaxResults)
            .ToList() ?? new ();

        return movies;
    }

    private RestRequest CreateRequest(string resource)
    {
        return new RestRequest(resource)
            .AddHeader("accept", "application/json")
            .AddHeader("Authorization", $"Bearer {_tmdbSettings.ApiKey}");
    }
}
