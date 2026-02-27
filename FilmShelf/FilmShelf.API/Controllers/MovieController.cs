using AutoMapper;
using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IMoviePageService _moviePageService;
    private readonly IReviewService _reviewService;
    private readonly IRecommendationService _recommendationService;
    private readonly IMapper _mapper;

    public MovieController(
        IMovieService movieService,
        IMoviePageService moviePageService,
        IReviewService reviewService,
        IRecommendationService recommendationService,
        IMapper mapper)
    {
        _movieService = movieService;
        _moviePageService = moviePageService;
        _reviewService = reviewService;
        _recommendationService = recommendationService;
        _mapper = mapper;
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
        IEnumerable<MovieDTO> movies;
        int? totalPages = null;

        if (!string.IsNullOrEmpty(pageRequestVM.Filter)
            && pageRequestVM.Filter.ToLower() == "popular")
        {
            movies = await _moviePageService.GetPopularMoviesPageAsync();
        }
        else
        {
            var page = pageRequestVM.Page ?? 1;
            (movies, totalPages) = await _moviePageService.GetMoviesOnPageAsync(page);
        }

        var movieListResponseVM = movies.ToMovieListResponseVM(totalPages);

        return Ok(movieListResponseVM);
    }

    [HttpGet("{movieId}/reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewVM>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RetrieveReviewsByMovieId(ReviewsForMovieVM reviewsForMovieVM)
    {
        var reviewDTOs = await _reviewService
            .GetReviewsByMovieIdAsync(reviewsForMovieVM.MovieId);

        var reviewVMs = reviewDTOs
            .Select(r => r.ToReviewVM());

        return Ok(reviewVMs);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchMovies(string searchQuery)
    {
        var movies = await _movieService.SearchMovie(searchQuery);

        return Ok(_mapper.Map<List<MovieResponseVM>>(movies));
    }

    [HttpGet("recommendations")]
    public async Task<IActionResult> GetMovieRecommendations()
    {
        var userId = UserClaimsHelper.GetUserId(User);
        
        var recommendedMovies = await _recommendationService
            .RecommendForUser(userId);

        if (recommendedMovies == null || !recommendedMovies.Any())
        {
            return NotFound();
        }

        return Ok(_mapper.Map<List<MovieResponseVM>>(recommendedMovies));
    }
}
