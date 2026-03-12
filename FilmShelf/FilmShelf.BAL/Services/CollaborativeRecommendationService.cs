using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmShelf.BAL.Services;

/// <summary>
/// User-based memory collaborative filtering using Pearson correlation.
/// Finds the K most similar users to the target, then predicts ratings for
/// movies those neighbors have seen but the target has not.
/// </summary>
public class CollaborativeRecommendationService : ICollaborativeRecommendationService
{
    private const int NeighborCount = 30;
    private const int MinSharedRatings = 2;

    private readonly FilmsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<CollaborativeRecommendationService> _logger;

    public CollaborativeRecommendationService(
        FilmsDbContext context,
        IMapper mapper,
        ILogger<CollaborativeRecommendationService> logger
    )
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<MovieDTO>> RecommendForUserAsync(int userId, int top = 10)
    {
        var allRatings = await _context
            .Reviews.Select(r => new
            {
                r.UserId,
                r.MovieId,
                r.Rating,
            })
            .ToListAsync();

        var userRatings = allRatings
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.ToDictionary(r => r.MovieId, r => (float)r.Rating));

        if (!userRatings.TryGetValue(userId, out var targetRatings))
            return new List<MovieDTO>();

        var targetMean = targetRatings.Values.Average();

        // Compute Pearson correlation with every other user that shares enough movies
        var similarities = new List<(int UserId, float Similarity)>();

        foreach (var (otherUserId, otherRatings) in userRatings)
        {
            if (otherUserId == userId)
                continue;

            var shared = targetRatings.Keys.Intersect(otherRatings.Keys).ToList();
            if (shared.Count < MinSharedRatings)
                continue;

            var sim = PearsonCorrelation(targetRatings, otherRatings, shared);
            if (sim > 0)
                similarities.Add((otherUserId, sim));
        }

        if (!similarities.Any())
            return new List<MovieDTO>();

        var neighbors = similarities
            .OrderByDescending(s => s.Similarity)
            .Take(NeighborCount)
            .ToList();

        // Predict ratings for movies seen by neighbors but not by target user
        // Formula: predicted = mean_target + sum(sim_i * (r_ij - mean_i)) / sum(|sim_i|)
        var ratedByTarget = targetRatings.Keys.ToHashSet();
        var candidateScores = new Dictionary<int, (float WeightedSum, float WeightSum)>();

        foreach (var (neighborId, similarity) in neighbors)
        {
            var neighborRatings = userRatings[neighborId];
            var neighborMean = neighborRatings.Values.Average();

            foreach (var (movieId, rating) in neighborRatings)
            {
                if (ratedByTarget.Contains(movieId))
                    continue;

                var entry = candidateScores.GetValueOrDefault(movieId);
                candidateScores[movieId] = (
                    entry.WeightedSum + similarity * (rating - neighborMean),
                    entry.WeightSum + MathF.Abs(similarity)
                );
            }
        }

        var topMovieIds = candidateScores
            .Where(kvp => kvp.Value.WeightSum > 0)
            .Select(kvp => new
            {
                MovieId = kvp.Key,
                PredictedRating = targetMean + kvp.Value.WeightedSum / kvp.Value.WeightSum,
            })
            .OrderByDescending(x => x.PredictedRating)
            .Take(top)
            .Select(x => x.MovieId)
            .ToList();

        if (!topMovieIds.Any())
            return new List<MovieDTO>();

        var movies = await _context.Movies.Where(m => topMovieIds.Contains(m.Id)).ToListAsync();

        _logger.LogInformation(
            "User-based CF: {Count} recommendations for user {UserId} using {Neighbors} neighbors",
            movies.Count,
            userId,
            neighbors.Count
        );

        return _mapper.Map<List<MovieDTO>>(movies);
    }

    /// <summary>
    /// Pearson correlation computed on shared movies only, using per-user means
    /// over those shared movies (standard formulation).
    /// </summary>
    private static float PearsonCorrelation(
        Dictionary<int, float> targetRatings,
        Dictionary<int, float> otherRatings,
        List<int> sharedMovies
    )
    {
        var targetMean = sharedMovies.Average(m => targetRatings[m]);
        var otherMean = sharedMovies.Average(m => otherRatings[m]);

        float numerator = 0,
            targetVar = 0,
            otherVar = 0;

        foreach (var movieId in sharedMovies)
        {
            var td = targetRatings[movieId] - targetMean;
            var od = otherRatings[movieId] - otherMean;
            numerator += td * od;
            targetVar += td * td;
            otherVar += od * od;
        }

        var denominator = MathF.Sqrt(targetVar * otherVar);
        return denominator < 1e-9f ? 0f : numerator / denominator;
    }
}
