using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IMoviePageService _moviePageService;

    public MoviesController(
        IMovieService movieService,
        IMoviePageService moviePageService)
    {
        _movieService = movieService;
        _moviePageService = moviePageService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MovieDetailsResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MovieDetailsById(MovieRequestVM movieRequestVM)
    {
        var movie = await _movieService.GetMovieAsync(movieRequestVM.Id);

        if (movie == null)
        {
            return NotFound();
        }

        var movieVM = movie.ToMovieDetailsResponseVM();

        return Ok(movieVM);
    }

    [HttpGet]
    [ProducesResponseType(typeof(MovieListResponseVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> MoviesOnPage(PageRequestVM pageRequestVM)
    {
        var (movies, totalPages) = await _moviePageService.GetMoviesOnPageAsync(pageRequestVM.Page);

        var movieListResponseVM = movies.ToMovieListResponseVM(totalPages);

        return Ok(movieListResponseVM);
    }
}
