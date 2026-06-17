using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MLModels;
using FilmShelf.DAL.Data;
using FilmShelf.TMDbClient.Options;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.BAL.Services;

public class RecommendationService : IRecommendationService
{
    private readonly FilmsDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMovieService _movieService;

    public RecommendationService(FilmsDbContext context, IMapper mapper, IMovieService movieService)
    {
        _context = context;
        _mapper = mapper;
        _movieService = movieService;
    }

    public async Task<List<MovieDTO>> RecommendForUser(int userId, int? holdOutMovieId = null, string language = LanguageConstants.English)
    {
        var ratings = await _context.Reviews
           .Select(r => new MovieRatingDTO
           {
               UserId = r.UserId,
               MovieId = r.MovieId,
               Label = r.Rating
           })
           .ToListAsync();

        var distinctUsers = ratings.Select(r => r.UserId).Distinct().Count();
        var distinctMovies = ratings.Select(r => r.MovieId).Distinct().Count();

        if (distinctUsers < 2 || distinctMovies < 2)
            return new List<MovieDTO>();

        var recommender = new MovieRecommender(ratings);
        var recommendedMovieIds = recommender.RecommendForUser(userId, ratings, top: 10, holdOutMovieId: holdOutMovieId);

        if (language != LanguageConstants.English)
            return await _movieService.GetLocalizedMoviesAsync(recommendedMovieIds, language);

        var recommendedMovies = await _context.Movies
            .Where(m => recommendedMovieIds.Contains(m.Id))
            .ToListAsync();

        return _mapper.Map<List<MovieDTO>>(recommendedMovies);
    }
}
