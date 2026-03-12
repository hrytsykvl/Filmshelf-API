using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.Options;
using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FilmShelf.BAL.Services;

public class MovieIndexService : IMovieIndexService
{
    private readonly FilmsDbContext _context;
    private readonly IAzureEmbeddingService _embeddingService;
    private readonly AzureSearchSettings _searchSettings;
    private readonly ILogger<MovieIndexService> _logger;

    public MovieIndexService(
        FilmsDbContext context,
        IAzureEmbeddingService embeddingService,
        IOptions<AzureSearchSettings> searchOptions,
        ILogger<MovieIndexService> logger)
    {
        _context = context;
        _embeddingService = embeddingService;
        _searchSettings = searchOptions.Value;
        _logger = logger;
    }

    public async Task IndexMoviesAsync()
    {
        await EnsureIndexExistsAsync();

        var movies = await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.MovieActors).ThenInclude(ma => ma.Actor)
            .ToListAsync();

        _logger.LogInformation("Indexing {Count} movies into Azure AI Search", movies.Count);

        var searchClient = new SearchClient(
            new Uri(_searchSettings.Endpoint),
            _searchSettings.IndexName,
            new AzureKeyCredential(_searchSettings.AdminApiKey));

        const int batchSize = 10;
        for (int i = 0; i < movies.Count; i += batchSize)
        {
            var batch = movies.Skip(i).Take(batchSize).ToList();
            var documents = new List<SearchDocument>();

            foreach (var movie in batch)
            {
                var text = BuildMovieText(movie);
                var embedding = await _embeddingService.GetEmbeddingAsync(text);
                documents.Add(new SearchDocument
                {
                    ["id"] = movie.Id.ToString(),
                    ["title"] = movie.Title,
                    ["contentVector"] = embedding
                });
            }

            await searchClient.MergeOrUploadDocumentsAsync(documents);
            _logger.LogInformation(
                "Indexed batch {Start}–{End} of {Total}",
                i + 1, Math.Min(i + batchSize, movies.Count), movies.Count);
        }

        _logger.LogInformation("Azure AI Search indexing complete");
    }

    private async Task EnsureIndexExistsAsync()
    {
        var indexClient = new SearchIndexClient(
            new Uri(_searchSettings.Endpoint),
            new AzureKeyCredential(_searchSettings.AdminApiKey));

        var index = new SearchIndex(_searchSettings.IndexName)
        {
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
                new SearchableField("title"),
                new VectorSearchField("contentVector", _searchSettings.VectorDimensions, "filmshelf-hnsw-profile")
            },
            VectorSearch = new VectorSearch
            {
                Algorithms = { new HnswAlgorithmConfiguration("filmshelf-hnsw") },
                Profiles = { new VectorSearchProfile("filmshelf-hnsw-profile", "filmshelf-hnsw") }
            }
        };

        await indexClient.CreateOrUpdateIndexAsync(index);
        _logger.LogInformation(
            "Azure AI Search index '{Index}' is ready", _searchSettings.IndexName);
    }

    private static string BuildMovieText(Movie movie)
    {
        var genres = string.Join(", ", movie.MovieGenres.Select(mg => mg.Genre.Name));
        var actors = string.Join(", ", movie.MovieActors.Take(5).Select(ma => ma.Actor.Name));
        return $"{movie.Title}. {movie.Overview}. Genres: {genres}. Director: {movie.Director.Name}. Cast: {actors}";
    }
}
