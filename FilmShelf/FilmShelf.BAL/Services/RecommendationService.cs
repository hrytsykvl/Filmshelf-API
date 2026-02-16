using AutoMapper;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MLModels;
using FilmShelf.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.BAL.Services;

public class RecommendationService : IRecommendationService
{
    private readonly FilmsDbContext _context;
    private readonly MovieRecommender _recommender;
    private readonly IMapper _mapper;


    public RecommendationService(FilmsDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        var ratings = _context.Reviews.Select(r => new MovieRatingDTO
        {
            UserId = r.UserId,
            MovieId = r.MovieId,
            Label = r.Rating
        }).ToList();

        _recommender = new MovieRecommender(ratings);
    }

    public async Task<List<MovieDTO>> RecommendForUser(int userId)
    {
        var ratings = await _context.Reviews
           .Select(r => new MovieRatingDTO
           {
               UserId = r.UserId,
               MovieId = r.MovieId,
               Label = r.Rating
           })
           .ToListAsync();

        var recommender = new MovieRecommender(ratings);

        var recommendedMovieIds = recommender.RecommendForUser(userId, ratings, top: 10);

        var recommendedMovies = await _context.Movies
            .Where(m => recommendedMovieIds.Contains(m.Id))
            .ToListAsync();

        return _mapper.Map<List<MovieDTO>>(recommendedMovies);
    }
}
