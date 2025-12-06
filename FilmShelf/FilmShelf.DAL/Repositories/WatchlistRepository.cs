using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class WatchlistRepository : IWatchlistRepository
{
    private readonly FilmsDbContext _context;

    public WatchlistRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task AddMovieToWatchlistAsync(WatchlistMovie watchlistMovie)
    {
        await _context.WatchlistMovies.AddAsync(watchlistMovie);
    }

    public async Task<(int WatchlistId, List<int> MovieIds)> GetDefaultWatchlistMoviesAsync(int userId)
    {
        var watchlist = await _context.Watchlists
            .Include(w => w.WatchlistMovies)
            .Where(w => w.IsDefault && w.UserId == userId)
            .Select(w => new
            {
                WatchlistId = w.Id,
                MovieIds = w.WatchlistMovies
                    .Select(wm => wm.MovieId)
                    .ToList()
            })
            .FirstAsync();

        return (watchlist.WatchlistId, watchlist.MovieIds);
    }

    public async Task<UserWatchlist?> GetWatchlistByIdAsync(int watchlistId)
    {
        return await _context.Watchlists
            .Include(w => w.WatchlistMovies)
            .ThenInclude(wm => wm.Movie)
            .FirstOrDefaultAsync(w => w.Id == watchlistId);
    }

    public async Task<WatchlistMovie?> GetWatchlistMovieAsync(int watchlistId, int movieId)
    {
        return await _context.WatchlistMovies
            .FirstOrDefaultAsync(wm => wm.WatchlistId == watchlistId && wm.MovieId == movieId);
    }

    public async Task RemoveMovieFromWatchlistAsync(int watchlistId, int movieId)
    {
        await _context.WatchlistMovies
            .Where(wm => wm.WatchlistId == watchlistId && wm.MovieId == movieId)
            .ExecuteDeleteAsync();
    }

    public async Task CreateWatchlistAsync(UserWatchlist watchlist)
    {
        await _context.Watchlists.AddAsync(watchlist);
    }

    public async Task DeleteWatchlistAsync(int watchlistId)
    {
        await _context.Watchlists
            .Where(w => w.Id == watchlistId)
            .ExecuteDeleteAsync();
    }
}
