using System.Collections.Generic;
using System.Diagnostics;
using AutoMapper;
using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IMoviePageService _moviePageService;
    private readonly IReviewService _reviewService;
    private readonly IRecommendationService _recommendationService;
    private readonly ILlmRecommendationService _llmRecommendationService;
    private readonly IContentBasedRecommendationService _contentBasedRecommendationService;
    private readonly ICollaborativeRecommendationService _collaborativeRecommendationService;
    private readonly IEmbeddingRecommendationService _embeddingRecommendationService;
    private readonly ILlamaRecommendationService _llamaRecommendationService;
    private readonly IMovieIndexService _movieIndexService;
    private readonly IMapper _mapper;
    private readonly ILogger<MovieController> _logger;

    public MovieController(
        IMovieService movieService,
        IMoviePageService moviePageService,
        IReviewService reviewService,
        IRecommendationService recommendationService,
        ILlmRecommendationService llmRecommendationService,
        IContentBasedRecommendationService contentBasedRecommendationService,
        ICollaborativeRecommendationService collaborativeRecommendationService,
        IEmbeddingRecommendationService embeddingRecommendationService,
        ILlamaRecommendationService llamaRecommendationService,
        IMovieIndexService movieIndexService,
        IMapper mapper,
        ILogger<MovieController> logger
    )
    {
        _movieService = movieService;
        _moviePageService = moviePageService;
        _reviewService = reviewService;
        _recommendationService = recommendationService;
        _llmRecommendationService = llmRecommendationService;
        _contentBasedRecommendationService = contentBasedRecommendationService;
        _collaborativeRecommendationService = collaborativeRecommendationService;
        _embeddingRecommendationService = embeddingRecommendationService;
        _llamaRecommendationService = llamaRecommendationService;
        _movieIndexService = movieIndexService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MovieDetailsResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MovieDetailsById(MovieRequestVM movieRequestVM)
    {
        var movie = await _movieService.GetMovieAsync(movieRequestVM.Id, movieRequestVM.Language);

        if (movie == null)
        {
            return NotFound();
        }

        var movieVM = movie.ToMovieDetailsResponseVM();

        return Ok(movieVM);
    }

    [HttpGet]
    [ProducesResponseType(typeof(MovieListResponseVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> MoviesOnPage(PageRequestVM pageRequestVM)
    {
        IEnumerable<MovieDTO> movies;
        int? totalPages = null;

        if (
            !string.IsNullOrEmpty(pageRequestVM.Filter)
            && pageRequestVM.Filter.ToLower() == "popular"
        )
        {
            movies = await _moviePageService.GetPopularMoviesPageAsync(pageRequestVM.Language);
        }
        else
        {
            var page = pageRequestVM.Page ?? 1;
            (movies, totalPages) = await _moviePageService.GetMoviesOnPageAsync(page, pageRequestVM.Language);
        }

        var movieListResponseVM = movies.ToMovieListResponseVM(totalPages);

        return Ok(movieListResponseVM);
    }

    [HttpGet("{movieId}/reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewVM>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetrieveReviewsByMovieId(ReviewsForMovieVM reviewsForMovieVM)
    {
        var reviewDTOs = await _reviewService.GetReviewsByMovieIdAsync(reviewsForMovieVM.MovieId);

        var reviewVMs = reviewDTOs.Select(r => r.ToReviewVM());

        return Ok(reviewVMs);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(string searchQuery, [FromQuery(Name = "language")] string language = "en-US")
    {
        var movies = await _movieService.SearchMovie(searchQuery, language);

        return Ok(_mapper.Map<List<MovieResponseVM>>(movies));
    }

    /// <summary>
    /// Import a batch of movies from TMDB by their IDs and save them to the database.
    /// Movies already in the database are skipped. Max 100 IDs per request.
    /// </summary>
    [HttpPost("import")]
    [Authorize]
    [ProducesResponseType(typeof(BulkImportResultVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkImportMovies([FromBody] BulkImportRequestVM request)
    {
        var result = await _movieService.BulkImportFromTmdbAsync(request.TmdbIds);

        return Ok(
            new BulkImportResultVM
            {
                Saved = result.Saved,
                Skipped = result.Skipped,
                Failed = result.Failed,
                FailedIds = result.FailedIds,
            }
        );
    }

    /// <summary>
    /// Import all movies from stored TMDB page snapshots (MoviePages table) into the database.
    /// IDs are extracted from every saved page automatically — no request body needed.
    /// Movies already in the database are skipped.
    /// </summary>
    [HttpPost("import/from-pages")]
    [Authorize]
    [ProducesResponseType(typeof(BulkImportResultVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkImportMoviesFromPages()
    {
        var result = await _movieService.BulkImportFromPagesAsync();

        return Ok(
            new BulkImportResultVM
            {
                Saved = result.Saved,
                Skipped = result.Skipped,
                Failed = result.Failed,
                FailedIds = result.FailedIds,
            }
        );
    }

    /// <summary>
    /// Get personalized movie recommendations.
    /// Supported methods:
    /// - llm (default): Claude-based with per-movie reasoning
    /// - ml: matrix factorization (ML.NET)
    /// - content: content-based filtering (genres, director, actors)
    /// - user-cf: user-based collaborative filtering (Pearson correlation)
    /// - embedding: Azure OpenAI embeddings + Azure AI Search vector similarity
    /// - llama: locally-hosted Llama model via Ollama with per-movie reasoning
    /// </summary>
    [HttpGet("recommendations")]
    [Authorize]
    public async Task<IActionResult> GetMovieRecommendations(
        [FromQuery] string method = "llm",
        [FromQuery(Name = "language")] string language = "en-US")
    {
        var userId = UserClaimsHelper.GetUserId(User);

        if (method.Equals("ml", StringComparison.OrdinalIgnoreCase))
        {
            var mlMovies = await _recommendationService.RecommendForUser(userId, language: language);

            if (mlMovies == null || !mlMovies.Any())
                return NotFound();

            return Ok(_mapper.Map<List<MovieResponseVM>>(mlMovies));
        }

        if (method.Equals("content", StringComparison.OrdinalIgnoreCase))
        {
            var contentMovies = await _contentBasedRecommendationService.RecommendForUserAsync(
                userId, language: language
            );

            if (!contentMovies.Any())
                return NotFound();

            return Ok(_mapper.Map<List<MovieResponseVM>>(contentMovies));
        }

        if (method.Equals("user-cf", StringComparison.OrdinalIgnoreCase))
        {
            var cfMovies = await _collaborativeRecommendationService.RecommendForUserAsync(userId, language: language);

            if (!cfMovies.Any())
                return NotFound();

            return Ok(_mapper.Map<List<MovieResponseVM>>(cfMovies));
        }

        if (method.Equals("embedding", StringComparison.OrdinalIgnoreCase))
        {
            var embeddingMovies = await _embeddingRecommendationService.RecommendForUserAsync(
                userId, language: language
            );

            if (!embeddingMovies.Any())
                return NotFound();

            return Ok(_mapper.Map<List<MovieResponseVM>>(embeddingMovies));
        }

        if (method.Equals("llama", StringComparison.OrdinalIgnoreCase))
        {
            var llamaRecs = await _llamaRecommendationService.RecommendForUserAsync(userId, language: language);

            if (!llamaRecs.Any())
                return NotFound();

            return Ok(llamaRecs.Select(r => r.ToLlmRecommendationVM()));
        }

        try
        {
            var llmRecs = await _llmRecommendationService.RecommendForUserAsync(userId, language: language);

            if (!llmRecs.Any())
                return NotFound();

            return Ok(llmRecs.Select(r => r.ToLlmRecommendationVM()));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "LLM recommendations failed for user {UserId}, falling back to ML",
                userId
            );

            var mlMovies = await _recommendationService.RecommendForUser(userId, language: language);

            if (mlMovies == null || !mlMovies.Any())
                return NotFound();

            return Ok(_mapper.Map<List<MovieResponseVM>>(mlMovies));
        }
    }

    /// <summary>
    /// Index all movies into Azure AI Search for embedding-based recommendations.
    /// Idempotent — safe to re-run to update existing documents.
    /// </summary>
    [HttpPost("index")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> IndexMovies()
    {
        await _movieIndexService.IndexMoviesAsync();
        return NoContent();
    }

    /// <summary>
    /// Get LLM-based movie recommendations with per-movie reasoning.
    /// Supported providers: claude (default), ollama.
    /// </summary>
    [HttpGet("recommendations/llm")]
    [Authorize]
    [ProducesResponseType(typeof(List<LlmRecommendationVM>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLlmRecommendations(
        [FromQuery] string provider = "claude",
        [FromQuery(Name = "language")] string language = "en-US")
    {
        var userId = UserClaimsHelper.GetUserId(User);

        List<LlmRecommendationDTO> recs;

        if (provider.Equals("ollama", StringComparison.OrdinalIgnoreCase))
        {
            recs = await _llamaRecommendationService.RecommendForUserAsync(userId, language: language);
        }
        else
        {
            recs = await _llmRecommendationService.RecommendForUserAsync(userId, language: language);
        }

        if (!recs.Any())
            return NotFound();

        return Ok(recs.Select(r => r.ToLlmRecommendationVM()));
    }

    /// <summary>
    /// Compare ML (matrix factorization) vs LLM-based recommendations side by side.
    /// </summary>
    [HttpGet("recommendations/compare")]
    [Authorize]
    [ProducesResponseType(typeof(ComparisonResultVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecommendationsComparison(
        [FromQuery(Name = "language")] string language = "en-US")
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var mlTask = _recommendationService.RecommendForUser(userId, language: language);
        var llmTask = _llmRecommendationService.RecommendForUserAsync(userId, language: language);

        await Task.WhenAll(mlTask, llmTask);

        var result = new ComparisonResultVM
        {
            MlRecommendations = _mapper.Map<List<MovieResponseVM>>(mlTask.Result),
            LlmRecommendations = llmTask.Result.Select(r => r.ToLlmRecommendationVM()).ToList(),
            GeneratedAt = DateTime.UtcNow,
        };

        return Ok(result);
    }

    /// <summary>
    /// Compare Claude vs Llama LLM-based recommendations side by side with response times.
    /// </summary>
    [HttpGet("recommendations/compare-llm")]
    [Authorize]
    [ProducesResponseType(typeof(LlmComparisonResultVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLlmComparison(
        [FromQuery(Name = "language")] string language = "en-US")
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var claudeSw = Stopwatch.StartNew();
        var claudeRecs = await _llmRecommendationService.RecommendForUserAsync(userId, language: language);
        claudeSw.Stop();

        var llamaSw = Stopwatch.StartNew();
        var llamaRecs = await _llamaRecommendationService.RecommendForUserAsync(userId, language: language);
        llamaSw.Stop();

        var result = new LlmComparisonResultVM
        {
            ClaudeRecommendations = claudeRecs.Select(r => r.ToLlmRecommendationVM()).ToList(),
            LlamaRecommendations = llamaRecs.Select(r => r.ToLlmRecommendationVM()).ToList(),
            ClaudeResponseTimeMs = claudeSw.Elapsed.TotalMilliseconds,
            LlamaResponseTimeMs = llamaSw.Elapsed.TotalMilliseconds,
            GeneratedAt = DateTime.UtcNow,
        };

        return Ok(result);
    }
}
