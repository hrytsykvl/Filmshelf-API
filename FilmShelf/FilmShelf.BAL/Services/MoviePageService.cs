using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Enums;
using FilmShelf.DAL.Interfaces;
using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace FilmShelf.BAL.Services;

public class MoviePageService : IMoviePageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieApiIntegrationService _movieApiIntegrationService;
    private readonly TmdbSettings _tmdbSettings;

    public MoviePageService(
        IUnitOfWork unitOfWork,
        IMovieApiIntegrationService movieApiIntegrationService,
        IOptions<TmdbSettings> tmdbSettings)
    {
        _unitOfWork = unitOfWork;
        _movieApiIntegrationService = movieApiIntegrationService;
        _tmdbSettings = tmdbSettings.Value;
    }

    public async Task FetchAndUpdateAsync()
    {
        for (int pageNumber = 1; pageNumber <= _tmdbSettings.PagesToFetch; pageNumber++)
        {
            var moviePage = await _unitOfWork.MoviePageRepository.GetPageAsync(pageNumber);

            if (ShouldUpdatePage(moviePage))
            {
                await FetchAndSavePageAsync(pageNumber);
            }
        }
    }

    public async Task<(IEnumerable<MovieDTO> Movies, int totalPages)> GetMoviesOnPageAsync(int pageNumber)
    {
        var moviePage = await _unitOfWork.MoviePageRepository.GetPageAsync(pageNumber);

        if (ShouldUpdatePage(moviePage))
        {
            moviePage = await FetchAndSavePageAsync(pageNumber);
        }

        if (moviePage == null)
        {
            return (Enumerable.Empty<MovieDTO>(), 0);
        }

        var movieResponse = JsonSerializer.Deserialize<PageResponseWrapperDTO>(moviePage.MoviesJson);

        return movieResponse == null
                ? (Enumerable.Empty<MovieDTO>(), 0)
                : (movieResponse.Results, movieResponse.TotalPages);
    }

    public async Task<IEnumerable<MovieDTO>> GetPopularMoviesPageAsync()
    {
        var popularMoviesPage = await _unitOfWork.MoviePageRepository
            .GetPageAsync(moviePageType: MoviePageType.Popular);

        if (ShouldUpdatePage(popularMoviesPage))
        {
            popularMoviesPage = await FetchAndSavePopularMoviesPageAsync();
        }

        if (popularMoviesPage == null)
        {
            return Enumerable.Empty<MovieDTO>();
        }

        var movieResponse = JsonSerializer
            .Deserialize<PageResponseWrapperDTO>(popularMoviesPage.MoviesJson);

        return movieResponse?.Results ?? Enumerable.Empty<MovieDTO>();
    }

    private async Task<MoviePage?> FetchAndSavePageAsync(int pageNumber)
    {
        var pageJson = await _movieApiIntegrationService.FetchMoviesPageAsync(pageNumber);

        if (pageJson == null)
        {
            return null;
        }

        var jsonDoc = JsonDocument.Parse(pageJson);
        var results = jsonDoc.RootElement.GetProperty("results");

        var combinedResult = new
        {
            results,
            total_pages = _tmdbSettings.TotalPages
        };

        var jsonResults = JsonSerializer.Serialize(combinedResult);

        var moviePage = new MoviePage
        {
            PageNumber = pageNumber,
            MoviesJson = jsonResults,
            UpdatedAt = DateTime.Now
        };

        var existingPage = await _unitOfWork.MoviePageRepository.GetPageAsync(moviePage.PageNumber);

        if (existingPage != null)
        {
            _unitOfWork.MoviePageRepository.UpdatePage(existingPage, moviePage);
        }
        else
        {
            await _unitOfWork.MoviePageRepository.AddPageAsync(moviePage);
        }

        await _unitOfWork.SaveAsync();
        return moviePage;
    }

    private async Task<MoviePage?> FetchAndSavePopularMoviesPageAsync()
    {
        var (jsonResponse, popularMovies) = await _movieApiIntegrationService
            .FetchPopularMoviesAsync(_tmdbSettings.PopularMoviesToFetch);

        if (string.IsNullOrEmpty(jsonResponse))
        {
            return null;
        }

        var moviePage = new MoviePage
        {
            PageNumber = 1,
            MoviesJson = jsonResponse,
            UpdatedAt = DateTime.UtcNow,
            Type = MoviePageType.Popular
        };

        var existingPage = await _unitOfWork.MoviePageRepository
            .GetPageAsync(moviePageType: MoviePageType.Popular);

        if (existingPage != null)
        {
            _unitOfWork.MoviePageRepository
                .UpdatePage(existingPage, moviePage);
        }
        else
        {
            await _unitOfWork.MoviePageRepository
                .AddPageAsync(moviePage);
        }

        await _unitOfWork.SaveAsync();

        return moviePage;
    }

    private bool ShouldUpdatePage(MoviePage? moviePage)
    {
        var todayScheduledTime = DateTime.Today.Add(TimeSpan.Parse(_tmdbSettings.ScheduledTime));

        return moviePage == null
            || moviePage.UpdatedAt.Date < DateTime.Today
            || moviePage.UpdatedAt < todayScheduledTime;
    }
}
