using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;

namespace FilmShelf.BAL.Services;

public class WatchlistService : IWatchlistService
{
    private readonly IUnitOfWork _unitOfWork;

    public WatchlistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateWatchlistAsync(
        int userId,
        string title,
        bool isDefault = false)
    {
        var watchlist = new UserWatchlist
        {
            UserId = userId,
            Title = title,
            IsDefault = isDefault
        };

        await _unitOfWork.WatchlistRepository.CreateWatchlistAsync(watchlist);
        await _unitOfWork.SaveAsync();

        return watchlist.Id;
    }

    public async Task AddMovieToWatchlistAsync(int watchlistId, int movieId)
    {
        var watchlistMovie = new WatchlistMovie
        {
            WatchlistId = watchlistId,
            MovieId = movieId,
            AddedAt = DateTime.UtcNow
        };

        await _unitOfWork.WatchlistRepository.AddMovieToWatchlistAsync(watchlistMovie);
        await _unitOfWork.SaveAsync();
    }

    public async Task<WatchlistDTO?> GetWatchlistByIdAsync(int watchlistId)
    {
        var watchlist = await _unitOfWork.WatchlistRepository
            .GetWatchlistByIdAsync(watchlistId);

        if (watchlist == null)
        {
            return null;
        }

        return new WatchlistDTO
        {
            Id = watchlist.Id,
            Title = watchlist.Title,
            TotalMovies = watchlist.WatchlistMovies?.Count ?? 0,
            UpdatedAt = watchlist.WatchlistMovies?.Any() ?? false
                ? watchlist.WatchlistMovies.Max(wm => wm.AddedAt)
                : null,
            Movies = watchlist.WatchlistMovies?.Select(wm => new WatchlistMovieDTO
            {
                Id = wm.MovieId,
                Title = wm.Movie.Title,
                PosterPath = wm.Movie.PosterPath,
                AverageRating = wm.Movie.AverageRating
            }).ToList() ?? new ()
        };
    }

    public async Task<List<WatchlistCheckDTO>> GetWatchlistMoviesAsync(int userId)
    {
        var userWatchlists = await _unitOfWork.WatchlistRepository
            .GetAllWatchlistMoviesAsync(userId);

        return userWatchlists
            .Select(w => new WatchlistCheckDTO
            {
                WatchlistId = w.WatchlistId,
                Title = w.Title,
                MovieIds = w.MovieIds
            }).ToList();
    }

    public async Task DeleteWatchlistAsync(int watchlistId)
    {
        await _unitOfWork.WatchlistRepository
            .DeleteWatchlistAsync(watchlistId);

        await _unitOfWork.SaveAsync();
    }

    public async Task RemoveMovieFromWatchlistAsync(int watchlistId, int movieId)
    {
        await _unitOfWork.WatchlistRepository
            .RemoveMovieFromWatchlistAsync(watchlistId, movieId);

        await _unitOfWork.SaveAsync();
    }

    public async Task UpdateWatchlistAsync(int watchlistId, string title)
    {
        await _unitOfWork.WatchlistRepository
            .UpdateWatchlistAsync(watchlistId, title);

        await _unitOfWork.SaveAsync();
    }
}
