using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IWatchlistRepository
{
    Task AddMovieToWatchlistAsync(WatchlistMovie watchlistMovie);
    Task<(int WatchlistId, List<int> MovieIds)> GetDefaultWatchlistMoviesAsync(int userId);
    Task<UserWatchlist?> GetWatchlistByIdAsync(int watchlistId);
    Task<WatchlistMovie?> GetWatchlistMovieAsync(int watchlistId, int movieId);
    Task RemoveMovieFromWatchlistAsync(int watchlistId, int movieId);
    Task CreateWatchlistAsync(UserWatchlist watchlist);
    Task DeleteWatchlistAsync(int watchlistId);
}
