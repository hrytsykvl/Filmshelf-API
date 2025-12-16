using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IWatchlistService
{
    Task<int> CreateWatchlistAsync(int userId, string title, bool isDefault = false);
    Task AddMovieToWatchlistAsync(int watchlistId, int movieId);
    Task<WatchlistDTO?> GetWatchlistByIdAsync(int watchlistId);
    Task<List<WatchlistCheckDTO>> GetWatchlistMoviesAsync(int userId);
    Task DeleteWatchlistAsync(int watchlistId);
    Task RemoveMovieFromWatchlistAsync(int watchlistId, int movieId);
    Task UpdateWatchlistAsync(int watchlistId, string title);
}
