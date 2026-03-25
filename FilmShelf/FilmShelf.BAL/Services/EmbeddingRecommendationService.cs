using System.Text;
using AutoMapper;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FilmShelf.BAL.Services;

public class EmbeddingRecommendationService : IEmbeddingRecommendationService
{
    private readonly FilmsDbContext _context;
    private readonly IAzureEmbeddingService _embeddingService;
    private readonly AzureSearchSettings _searchSettings;
    private readonly IMapper _mapper;
    private readonly ILogger<EmbeddingRecommendationService> _logger;

    public EmbeddingRecommendationService(
        FilmsDbContext context,
        IAzureEmbeddingService embeddingService,
        IOptions<AzureSearchSettings> searchOptions,
        IMapper mapper,
        ILogger<EmbeddingRecommendationService> logger
    )
    {
        _context = context;
        _embeddingService = embeddingService;
        _searchSettings = searchOptions.Value;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MovieDTO>> RecommendForUserAsync(int userId, int top = 10, int? holdOutMovieId = null)
    {
        var userReviews = await _context
            .Reviews.Include(r => r.Movie)
                .ThenInclude(m => m.Director)
            .Include(r => r.Movie)
                .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
            .Include(r => r.Movie)
                .ThenInclude(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        var ratedMovieIds = userReviews.Select(r => r.MovieId).ToHashSet();

        var watchlistedMovieIds = await _context
            .Watchlists.Where(w => w.UserId == userId)
            .SelectMany(w => w.WatchlistMovies.Select(wm => wm.MovieId))
            .ToListAsync();

        var excludedIds = ratedMovieIds.Concat(watchlistedMovieIds).ToHashSet();
        if (holdOutMovieId.HasValue)
            excludedIds.Remove(holdOutMovieId.Value);

        var profileText = BuildUserProfile(userReviews);
        var profileEmbedding = await _embeddingService.GetEmbeddingAsync(profileText);

        _logger.LogInformation(
            "Querying Azure AI Search for user {UserId} ({ReviewCount} reviews)",
            userId,
            userReviews.Count
        );

        var searchClient = new SearchClient(
            new Uri(_searchSettings.Endpoint),
            _searchSettings.IndexName,
            new AzureKeyCredential(_searchSettings.QueryApiKey)
        );

        var fetchCount = Math.Min(top + excludedIds.Count + 10, 100);

        var vectorQuery = new VectorizedQuery(profileEmbedding)
        {
            KNearestNeighborsCount = fetchCount,
        };
        vectorQuery.Fields.Add("contentVector");

        var searchOptions = new SearchOptions { Size = fetchCount };
        searchOptions.VectorSearch = new VectorSearchOptions();
        searchOptions.VectorSearch.Queries.Add(vectorQuery);

        var searchResponse = await searchClient.SearchAsync<SearchDocument>(
            searchText: null,
            options: searchOptions
        );

        var recommendedMovieIds = new List<int>();
        await foreach (var result in searchResponse.Value.GetResultsAsync())
        {
            if (
                result.Document.TryGetValue("id", out var idObj)
                && int.TryParse(idObj?.ToString(), out var movieId)
                && !excludedIds.Contains(movieId)
            )
            {
                recommendedMovieIds.Add(movieId);
                if (recommendedMovieIds.Count >= top)
                    break;
            }
        }

        if (!recommendedMovieIds.Any())
        {
            _logger.LogInformation("No embedding recommendations found for user {UserId}", userId);
            return new List<MovieDTO>();
        }

        var movies = await _context
            .Movies.Include(m => m.Director)
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
            .Where(m => recommendedMovieIds.Contains(m.Id))
            .ToListAsync();

        // Preserve vector search ranking order
        return recommendedMovieIds
            .Select(id => movies.FirstOrDefault(m => m.Id == id))
            .Where(m => m != null)
            .Select(m => _mapper.Map<MovieDTO>(m!))
            .ToList();
    }

    private static string BuildUserProfile(List<Review> userReviews)
    {
        if (!userReviews.Any())
            return "General audience. Recommend highly rated and critically acclaimed films.";

        var highRated = userReviews.Where(r => r.Rating >= 7).ToList();

        var favoriteGenres = highRated
            .SelectMany(r => r.Movie.MovieGenres.Select(mg => mg.Genre.Name))
            .GroupBy(g => g)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => g.Key);

        var favoriteDirectors = highRated
            .Select(r => r.Movie.Director.Name)
            .GroupBy(d => d)
            .OrderByDescending(d => d.Count())
            .Take(3)
            .Select(d => d.Key);

        var favoriteActors = highRated
            .SelectMany(r => r.Movie.MovieActors.Select(ma => ma.Actor.Name))
            .GroupBy(a => a)
            .OrderByDescending(a => a.Count())
            .Take(5)
            .Select(a => a.Key);

        var topRated = userReviews
            .OrderByDescending(r => r.Rating)
            .Take(10)
            .Select(r => $"{r.Movie.Title} ({r.Rating}/10)");

        var sb = new StringBuilder();
        sb.AppendLine($"Favorite genres: {string.Join(", ", favoriteGenres)}");
        sb.AppendLine($"Favorite directors: {string.Join(", ", favoriteDirectors)}");
        sb.AppendLine($"Favorite actors: {string.Join(", ", favoriteActors)}");
        sb.AppendLine($"Top rated movies: {string.Join(", ", topRated)}");
        sb.AppendLine($"Total movies rated: {userReviews.Count}");

        return sb.ToString();
    }
}
