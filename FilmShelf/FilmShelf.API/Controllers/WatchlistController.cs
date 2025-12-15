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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateWatchlist([FromBody] UpsertWatchlistVM upsertWatchlistVM)
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var createdWatchlistId = await _watchlistService.CreateWatchlistAsync(
            userId,
            upsertWatchlistVM.Title);

        return CreatedAtAction(
            nameof(RetrieveWatchlist),
            new { id = createdWatchlistId },
            createdWatchlistId);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateWatchlist(
        WatchlistRequestVM watchlistRequestVM,
        [FromBody] UpsertWatchlistVM upsertWatchlistVM)
    {
        await _watchlistService.UpdateWatchlistAsync(
            watchlistRequestVM.WatchlistId,
            upsertWatchlistVM.Title);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteWatchlist(WatchlistRequestVM watchlistRequestVM)
    {
        await _watchlistService
            .DeleteWatchlistAsync(watchlistRequestVM.WatchlistId);

        return NoContent();
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

        await _watchlistService.RemoveMovieFromWatchlistAsync(
            watchlistActionVM.WatchlistId,
            watchlistActionVM.MovieId);

        return NoContent();
    }

    [HttpGet("movies")]
    [ProducesResponseType(typeof(IEnumerable<WatchlistCheckVM>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MoviesInWatchlists()
    {
        var userId = UserClaimsHelper.GetUserId(User);
        var watchlistCheckDTOs = await _watchlistService
            .GetWatchlistMoviesAsync(userId);

        var watchlistCheckVMs = watchlistCheckDTOs.ToWatchlistCheckVMs();

        return Ok(watchlistCheckVMs);
    }
}
