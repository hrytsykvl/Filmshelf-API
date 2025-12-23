using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IReviewRepository
{
    Task AddReviewAsync(Review review);
    Task<List<Review>> GetReviewsByMovieIdAsync(int movieId);
    Task<List<Review>> GetReviewsByUserIdAsync(int userId);
    Task<Review?> GetReviewByIdAsync(int reviewId);
    Task UpdateReviewAsync(int reviewId, string content, byte rating);
    Task DeleteReviewAsync(int reviewId);
    Task AddReviewResponseAsync(ReviewResponse reviewResponse);
    Task<ReviewResponse?> GetResponseByIdAsync(int responseId);
    Task<List<ReviewResponse>> GetResponsesByReviewIdAsync(int reviewId);
    Task DeleteReviewResponseAsync(int reviewResponseId, int userId);
}
