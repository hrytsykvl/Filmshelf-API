using FilmShelf.DAL.Data;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class EvaluationRepository : IEvaluationRepository
{
    private readonly FilmsDbContext _context;

    public EvaluationRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<(int UserId, int MovieId, byte Rating, DateTime CreatedAt)>> GetAllReviewsAsync()
    {
        return await _context.Reviews
            .Select(r => ValueTuple.Create(r.UserId, r.MovieId, r.Rating, r.CreatedAt))
            .ToListAsync();
    }

    public async Task<List<int>> GetUserIdsWithMinReviewsAsync(int minReviews)
    {
        return await _context.Reviews
            .GroupBy(r => r.UserId)
            .Where(g => g.Count() >= minReviews)
            .Select(g => g.Key)
            .ToListAsync();
    }

    public async Task<int> GetTotalMovieCountAsync()
    {
        return await _context.Movies.CountAsync();
    }
}
