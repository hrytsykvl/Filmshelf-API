using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.TMDbClient.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilmShelf.BAL.Services;

public class ContentBasedRecommendationService : IContentBasedRecommendationService
{
    private readonly FilmsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ContentBasedRecommendationService> _logger;
    private readonly IMovieService _movieService;

    public ContentBasedRecommendationService(
        FilmsDbContext context,
        IMapper mapper,
        ILogger<ContentBasedRecommendationService> logger,
        IMovieService movieService
    )
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _movieService = movieService;
    }

    public async Task<List<MovieDTO>> RecommendForUserAsync(int userId, int top = 10, int? holdOutMovieId = null, string language = LanguageConstants.English)
    {
        var userReviews = await _context
            .Reviews.Where(r => r.UserId == userId)
            .Select(r => new { r.MovieId, r.Rating })
            .ToListAsync();

        if (!userReviews.Any())
            return new List<MovieDTO>();

        var ratedMovieIds = userReviews.Select(r => r.MovieId).ToHashSet();
        if (holdOutMovieId.HasValue)
            ratedMovieIds.Remove(holdOutMovieId.Value);

        var allMovies = await _context
            .Movies.Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
            .Include(m => m.Director)
            .Include(m => m.MovieActors)
                .ThenInclude(ma => ma.Actor)
            .ToListAsync();

        var featureIndex = BuildFeatureIndex(allMovies);
        var featureCount = featureIndex.Count;

        var movieVectors = allMovies.ToDictionary(
            m => m.Id,
            m => BuildNormalizedMovieVector(m, featureIndex, featureCount)
        );

        // Weighted sum of rated movie vectors; weight = rating / 10
        var profile = new float[featureCount];
        foreach (var review in userReviews)
        {
            if (!movieVectors.TryGetValue(review.MovieId, out var movieVector))
                continue;

            var weight = review.Rating / 10f;
            for (int i = 0; i < featureCount; i++)
                profile[i] += weight * movieVector[i];
        }

        var profileNorm = L2Norm(profile);
        if (profileNorm < 1e-9f)
            return new List<MovieDTO>();

        var candidates = allMovies
            .Where(m => !ratedMovieIds.Contains(m.Id))
            .Select(m => new
            {
                Movie = m,
                // movie vector is unit-length, so cosine = dot / profileNorm
                Score = Dot(profile, movieVectors[m.Id]) / profileNorm,
            })
            .OrderByDescending(x => x.Score)
            .Take(top)
            .Select(x => x.Movie)
            .ToList();

        _logger.LogInformation(
            "Content-based: {Count} recommendations for user {UserId} (feature space: {FeatureCount})",
            candidates.Count,
            userId,
            featureCount
        );

        if (language != LanguageConstants.English)
            return await _movieService.GetLocalizedMoviesAsync(candidates.Select(m => m.Id), language);

        return _mapper.Map<List<MovieDTO>>(candidates);
    }

    private static Dictionary<string, int> BuildFeatureIndex(List<Movie> movies)
    {
        var index = new Dictionary<string, int>();

        foreach (var movie in movies)
        {
            foreach (var mg in movie.MovieGenres)
                TryAdd(index, $"genre:{mg.Genre.Name}");

            TryAdd(index, $"director:{movie.Director.Name}");

            foreach (var ma in movie.MovieActors)
                TryAdd(index, $"actor:{ma.Actor.Name}");
        }

        return index;
    }

    private static void TryAdd(Dictionary<string, int> index, string key)
    {
        if (!index.ContainsKey(key))
            index[key] = index.Count;
    }

    private static float[] BuildNormalizedMovieVector(
        Movie movie,
        Dictionary<string, int> featureIndex,
        int featureCount
    )
    {
        var vector = new float[featureCount];

        foreach (var mg in movie.MovieGenres)
        {
            if (featureIndex.TryGetValue($"genre:{mg.Genre.Name}", out var idx))
                vector[idx] = 1f;
        }

        if (featureIndex.TryGetValue($"director:{movie.Director.Name}", out var dirIdx))
            vector[dirIdx] = 1f;

        foreach (var ma in movie.MovieActors)
        {
            if (featureIndex.TryGetValue($"actor:{ma.Actor.Name}", out var actIdx))
                vector[actIdx] = 1f;
        }

        var norm = L2Norm(vector);
        if (norm > 1e-9f)
            for (int i = 0; i < featureCount; i++)
                vector[i] /= norm;

        return vector;
    }

    private static float L2Norm(float[] v)
    {
        float sum = 0;
        foreach (var x in v)
            sum += x * x;
        return MathF.Sqrt(sum);
    }

    private static float Dot(float[] a, float[] b)
    {
        float sum = 0;
        for (int i = 0; i < a.Length; i++)
            sum += a[i] * b[i];
        return sum;
    }
}
