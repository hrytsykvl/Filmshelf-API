using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilmShelf.BAL.Services;

public class LlmRecommendationService : ILlmRecommendationService
{
    private readonly FilmsDbContext _context;
    private readonly IClaudeApiService _claudeApiService;
    private readonly IMapper _mapper;
    private readonly ILogger<LlmRecommendationService> _logger;

    public LlmRecommendationService(
        FilmsDbContext context,
        IClaudeApiService claudeApiService,
        IMapper mapper,
        ILogger<LlmRecommendationService> logger)
    {
        _context = context;
        _claudeApiService = claudeApiService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<LlmRecommendationDTO>> RecommendForUserAsync(int userId, int top = 10)
    {
        var userReviews = await _context.Reviews
            .Include(r => r.Movie)
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

        var watchlistedMovieIds = await _context.Watchlists
            .Where(w => w.UserId == userId)
            .SelectMany(w => w.WatchlistMovies.Select(wm => wm.MovieId))
            .ToListAsync();

        var excludedIds = ratedMovieIds.Concat(watchlistedMovieIds).ToHashSet();

        var candidates = await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
            .Where(m => !excludedIds.Contains(m.Id))
            .OrderByDescending(m => m.AverageRating)
            .Take(50)
            .ToListAsync();

        if (!candidates.Any())
        {
            _logger.LogInformation("No candidate movies found for user {UserId}", userId);
            return new List<LlmRecommendationDTO>();
        }

        var userProfile = BuildUserProfile(userReviews);
        var candidatesJson = BuildCandidatesJson(candidates);

        _logger.LogInformation(
            "Built profile for user {UserId}: {ReviewCount} reviews, {CandidateCount} candidates",
            userId, userReviews.Count, candidates.Count);

        var systemPrompt =
            "You are a movie recommendation expert. " +
            "Given a user's taste profile and a list of candidate movies, " +
            "score each candidate from 0 to 10 based on predicted user enjoyment. " +
            "Return ONLY a valid JSON array with no additional text, markdown, or explanation.";

        var userMessage = new StringBuilder();
        userMessage.AppendLine("## User Taste Profile");
        userMessage.AppendLine(userProfile);
        userMessage.AppendLine();
        userMessage.AppendLine("## Candidate Movies");
        userMessage.AppendLine(candidatesJson);
        userMessage.AppendLine();
        userMessage.AppendLine(
            "Return a JSON array only — no markdown, no text before or after: " +
            "[{\"movieId\": 1, \"score\": 8.5, \"reason\": \"one or two sentence explanation\"}, ...]");

        var claudeResponse = await _claudeApiService.SendMessageAsync(
            systemPrompt, userMessage.ToString());

        _logger.LogInformation("Received Claude response for user {UserId}", userId);

        return ParseClaudeResponse(claudeResponse, candidates, top);
    }

    private string BuildUserProfile(List<Review> userReviews)
    {
        if (!userReviews.Any())
            return "New user with no rating history. Recommend highly rated and critically acclaimed films.";

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

    private string BuildCandidatesJson(List<Movie> candidates)
    {
        var items = candidates.Select(m => new
        {
            id = m.Id,
            title = m.Title,
            director = m.Director.Name,
            genres = m.MovieGenres.Select(mg => mg.Genre.Name).ToList(),
            overview = m.Overview.Length > 200 ? m.Overview[..200] + "..." : m.Overview,
            averageRating = m.AverageRating,
            releaseYear = m.ReleaseDate.Year
        });

        return JsonSerializer.Serialize(items);
    }

    private List<LlmRecommendationDTO> ParseClaudeResponse(
        string response, List<Movie> candidates, int top)
    {
        var json = ExtractJson(response);
        var candidateDict = candidates.ToDictionary(m => m.Id);

        List<ClaudeRecommendationItem> scoredItems;
        try
        {
            scoredItems = JsonSerializer.Deserialize<List<ClaudeRecommendationItem>>(json)
                ?? new List<ClaudeRecommendationItem>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse Claude JSON response: {Response}", response);
            throw new InvalidOperationException("Claude returned an invalid JSON response.", ex);
        }

        return scoredItems
            .Where(item => candidateDict.ContainsKey(item.MovieId))
            .OrderByDescending(item => item.Score)
            .Take(top)
            .Select(item => new LlmRecommendationDTO
            {
                Movie = _mapper.Map<MovieDTO>(candidateDict[item.MovieId]),
                Score = item.Score,
                Reason = item.Reason
            })
            .ToList();
    }

    private static string ExtractJson(string response)
    {
        response = response.Trim();

        if (response.StartsWith("```"))
        {
            var newline = response.IndexOf('\n');
            var closing = response.LastIndexOf("```");
            if (newline >= 0 && closing > newline)
                return response[(newline + 1)..closing].Trim();
        }

        return response;
    }

    private sealed class ClaudeRecommendationItem
    {
        [JsonPropertyName("movieId")]
        public int MovieId { get; set; }

        [JsonPropertyName("score")]
        public float Score { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;
    }
}
