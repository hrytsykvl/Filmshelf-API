using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IWatchlistService
{
    Task CreateWatchlistAsync(int userId, string title, bool isDefault = false);
    Task AddMovieToWatchlistAsync(int watchlistId, int movieId);
    Task<WatchlistDTO?> GetWatchlistByIdAsync(int watchlistId);
    Task<WatchlistCheckDTO> GetDefaultWatchlistMoviesAsync(int userId);
    Task DeleteWatchlistAsync(int watchlistId);
    Task RemoveFromWatchlistAsync(int watchlistId, int movieId);
}
