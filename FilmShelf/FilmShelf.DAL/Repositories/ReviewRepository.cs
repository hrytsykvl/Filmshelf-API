using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly FilmsDbContext _context;

    public ReviewRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task AddReviewAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task AddReviewResponseAsync(ReviewResponse reviewResponse)
    {
        await _context.ReviewResponses.AddAsync(reviewResponse);
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        await _context.Reviews
            .Where(r => r.Id == reviewId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteReviewResponseAsync(int reviewResponseId, int userId)
    {
        await _context.ReviewResponses
            .Where(r => r.Id == reviewResponseId && r.UserId == userId)
            .ExecuteDeleteAsync();
    }

    public async Task<Review?> GetReviewByIdAsync(int reviewId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Movie)
            .Include(r => r.ReviewResponses)
            .FirstOrDefaultAsync(r => r.Id == reviewId);
    }

    public async Task<List<Review>> GetReviewsByMovieIdAsync(int movieId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Movie)
            .Include(r => r.ReviewResponses)
            .ThenInclude(rr => rr.User)
            .Where(r => r.MovieId == movieId)
            .ToListAsync();
    }

    public async Task<List<Review>> GetReviewsByUserIdAsync(int userId)
    {
        return await _context.Reviews
            .Include(r => r.Movie)
            .Include(r => r.User)
            .Include(r => r.ReviewResponses)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<ReviewResponse?> GetResponseByIdAsync(int responseId)
    {
        return await _context.ReviewResponses
            .Include(r => r.User)
            .Include(r => r.Review)
            .FirstOrDefaultAsync(r => r.Id == responseId);
    }

    public async Task<List<ReviewResponse>> GetResponsesByReviewIdAsync(int reviewId)
    {
        return await _context.ReviewResponses
            .Include(r => r.User)
            .Where(r => r.ReviewId == reviewId)
            .ToListAsync();
    }

    public async Task UpdateReviewAsync(int reviewId, string content, byte rating)
    {
        await _context.Reviews
            .Where(r => r.Id == reviewId)
            .ExecuteUpdateAsync(r => r
                .SetProperty(t => t.Content, content)
                .SetProperty(t => t.Rating, rating));
    }
}
