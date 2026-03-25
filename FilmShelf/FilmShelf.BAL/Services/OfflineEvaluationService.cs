using System.Diagnostics;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace FilmShelf.BAL.Services;

public class OfflineEvaluationService(
    IRecommendationService mlService,
    IContentBasedRecommendationService contentService,
    ICollaborativeRecommendationService cfService,
    ILlmRecommendationService llmService,
    ILlamaRecommendationService llamaService,
    IEmbeddingRecommendationService embeddingService,
    IEvaluationRepository evaluationRepository,
    ILogger<OfflineEvaluationService> logger
) : IOfflineEvaluationService
{
    private static readonly string[] AllMethods = ["ml", "content", "user-cf", "llm", "llama", "embedding"];

    public async Task<List<EvaluationResultDTO>> EvaluateAllMethodsAsync(
        EvaluationRequestDTO request
    )
    {
        var results = new List<EvaluationResultDTO>();
        foreach (var method in AllMethods)
        {
            logger.LogInformation("Starting evaluation for method '{Method}'", method);
            results.Add(await EvaluateMethodAsync(method, request));
        }
        return results;
    }

    public async Task<EvaluationResultDTO> EvaluateMethodAsync(
        string method,
        EvaluationRequestDTO request
    )
    {
        if (!AllMethods.Contains(method, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException(
                $"Unknown recommendation method '{method}'. Valid values: {string.Join(", ", AllMethods)}"
            );

        var sw = Stopwatch.StartNew();

        var eligibleUserIds = await evaluationRepository.GetUserIdsWithMinReviewsAsync(
            request.MinReviews
        );
        var totalMovies = await evaluationRepository.GetTotalMovieCountAsync();

        var allReviews = await evaluationRepository.GetAllReviewsAsync();
        var reviewsByUser = allReviews
            .GroupBy(r => r.UserId)
            .ToDictionary(g => g.Key, g => g.OrderBy(r => r.CreatedAt).ToList());

        var hits = new List<double>();
        var precisions = new List<double>();
        var ndcgs = new List<double>();
        var reciprocalRanks = new List<double>();
        var recommendedMovieIds = new HashSet<int>();
        var skippedUsers = 0;

        var usersToEvaluate = request.LlmMaxUsers.HasValue &&
            (method.Equals("llm", StringComparison.OrdinalIgnoreCase) ||
             method.Equals("llama", StringComparison.OrdinalIgnoreCase) ||
             method.Equals("embedding", StringComparison.OrdinalIgnoreCase))
            ? eligibleUserIds.Take(request.LlmMaxUsers.Value)
            : eligibleUserIds;

        foreach (var userId in usersToEvaluate)
        {
            if (
                !reviewsByUser.TryGetValue(userId, out var userReviews)
                || userReviews.Count < request.MinReviews
            )
                continue;

            var testItem = userReviews.Last();

            // Skip if the user didn't positively rate the test item
            if (testItem.Rating < request.RelevanceThreshold)
                continue;

            List<int> recommendedIds;
            try
            {
                recommendedIds = await GetRecommendedIdsAsync(method, userId, request.K, testItem.MovieId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Recommender '{Method}' failed for user {UserId}, skipping",
                    method,
                    userId
                );
                skippedUsers++;
                continue;
            }

            if (recommendedIds.Count == 0)
            {
                skippedUsers++;
                continue;
            }

            foreach (var id in recommendedIds)
                recommendedMovieIds.Add(id);

            var rank = recommendedIds.IndexOf(testItem.MovieId) + 1; // 1-based, 0 means not found

            double hit = rank > 0 ? 1.0 : 0.0;
            double precision = hit / request.K;
            double ndcg = rank > 0 ? 1.0 / Math.Log2(rank + 1) : 0.0;
            double mrr = rank > 0 ? 1.0 / rank : 0.0;

            hits.Add(hit);
            precisions.Add(precision);
            ndcgs.Add(ndcg);
            reciprocalRanks.Add(mrr);
        }

        sw.Stop();

        int evaluatedUsers = hits.Count;
        logger.LogInformation(
            "Evaluation complete for '{Method}': {Evaluated} users evaluated, {Skipped} skipped, {Duration:F1}s",
            method,
            evaluatedUsers,
            skippedUsers,
            sw.Elapsed.TotalSeconds
        );

        return new EvaluationResultDTO
        {
            Method = method.ToLowerInvariant(),
            K = request.K,
            EvaluatedUsers = evaluatedUsers,
            SkippedUsers = skippedUsers,
            HitRateAtK = evaluatedUsers > 0 ? hits.Average() : 0,
            PrecisionAtK = evaluatedUsers > 0 ? precisions.Average() : 0,
            RecallAtK = evaluatedUsers > 0 ? hits.Average() : 0, // identical to HitRate when |test set| = 1
            NdcgAtK = evaluatedUsers > 0 ? ndcgs.Average() : 0,
            Mrr = evaluatedUsers > 0 ? reciprocalRanks.Average() : 0,
            CatalogCoverage = totalMovies > 0 ? (double)recommendedMovieIds.Count / totalMovies : 0,
            EvaluationDurationSeconds = sw.Elapsed.TotalSeconds,
            EvaluatedAt = DateTime.UtcNow,
        };
    }

    private async Task<List<int>> GetRecommendedIdsAsync(string method, int userId, int k, int holdOutMovieId)
    {
        if (method.Equals("ml", StringComparison.OrdinalIgnoreCase))
        {
            var movies = await mlService.RecommendForUser(userId, holdOutMovieId);
            return movies?.Take(k).Select(m => m.Id).ToList() ?? [];
        }

        if (method.Equals("content", StringComparison.OrdinalIgnoreCase))
        {
            var movies = await contentService.RecommendForUserAsync(userId, k, holdOutMovieId);
            return movies.Select(m => m.Id).ToList();
        }

        if (method.Equals("user-cf", StringComparison.OrdinalIgnoreCase))
        {
            var movies = await cfService.RecommendForUserAsync(userId, k, holdOutMovieId);
            return movies.Select(m => m.Id).ToList();
        }

        if (method.Equals("llm", StringComparison.OrdinalIgnoreCase))
        {
            var recs = await llmService.RecommendForUserAsync(userId, k, holdOutMovieId);
            return recs.Select(r => r.Movie.Id).ToList();
        }

        if (method.Equals("llama", StringComparison.OrdinalIgnoreCase))
        {
            var recs = await llamaService.RecommendForUserAsync(userId, k, holdOutMovieId);
            return recs.Select(r => r.Movie.Id).ToList();
        }

        if (method.Equals("embedding", StringComparison.OrdinalIgnoreCase))
        {
            var movies = await embeddingService.RecommendForUserAsync(userId, k, holdOutMovieId);
            return movies.Select(m => m.Id).ToList();
        }

        return [];
    }
}
