using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IReviewService
{
    Task<ReviewDTO> AddReviewAsync(ReviewAddDTO reviewAddDTO);
    Task<List<ReviewDTO>> GetReviewsByMovieIdAsync(int movieId);
    Task<List<ReviewDTO>> GetReviewsByUserIdAsync(int userId);
    Task<ReviewDTO?> GetReviewByIdAsync(int reviewId);
    Task UpdateReviewAsync(int reviewId, string content, byte rating);
    Task DeleteReviewAsync(int reviewId);
    Task<ReviewResponseDTO> AddReviewResponseAsync(ReviewResponseAddDTO reviewResponseAddDTO);
    Task<ReviewResponseDTO?> GetReviewResponseByIdAsync(int responseId);
    Task<List<ReviewResponseDTO>> GetReviewResponsesByReviewIdAsync(int reviewId);
    Task DeleteReviewResponseAsync(int reviewResponseId, int userId);
}
