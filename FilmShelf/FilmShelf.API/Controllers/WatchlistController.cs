using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;

    public WatchlistController(IWatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    [HttpPut("{watchlistId}/movies/{movieId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddMovieToWatchlist(WatchlistAddVM watchlistAddVM)
    {
        await _watchlistService.AddMovieToWatchlistAsync(
            watchlistAddVM.WatchlistId,
            watchlistAddVM.MovieId);

        return NoContent();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WatchlistVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetrieveWatchlist(WatchlistRequestVM watchlistRequestVM)
    {
        var watchlist = await _watchlistService
            .GetWatchlistByIdAsync(watchlistRequestVM.WatchlistId);

        if (watchlist == null)
        {
            return NotFound();
        }

        var watchlistVM = watchlist.ToWatchlistVM();

        return Ok(watchlistVM);
    }

    [HttpDelete("{watchlistId}/movies/{movieId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveMovieFromWatchlist(WatchlistActionVM watchlistActionVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);

        await _watchlistService.RemoveFromWatchlistAsync(
            watchlistActionVM.WatchlistId,
            watchlistActionVM.MovieId);

        return NoContent();
    }

    [HttpGet("default/movies")]
    [ProducesResponseType(typeof(WatchlistCheckVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> MoviesInDefaultWatchlist()
    {
        var userId = UserClaimsHelper.GetUserId(User);
        var watchlistCheckDTO = await _watchlistService
            .GetDefaultWatchlistMoviesAsync(userId);

        var watchlistCheckVM = watchlistCheckDTO.ToWatchlistCheckVM();

        return Ok(watchlistCheckVM);
    }
}
