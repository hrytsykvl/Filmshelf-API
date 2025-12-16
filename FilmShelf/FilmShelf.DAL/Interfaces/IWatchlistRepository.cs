using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IWatchlistRepository
{
    Task AddMovieToWatchlistAsync(WatchlistMovie watchlistMovie);
    Task<List<(int WatchlistId, string Title, List<int> MovieIds)>> GetAllWatchlistMoviesAsync(int userId);
    Task<UserWatchlist?> GetWatchlistByIdAsync(int watchlistId);
    Task<WatchlistMovie?> GetWatchlistMovieAsync(int watchlistId, int movieId);
    Task RemoveMovieFromWatchlistAsync(int watchlistId, int movieId);
    Task CreateWatchlistAsync(UserWatchlist watchlist);
    Task UpdateWatchlistAsync(int watchlistId, string title);
    Task DeleteWatchlistAsync(int watchlistId);
}
