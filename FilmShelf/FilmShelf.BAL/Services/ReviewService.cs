using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MappingExtensions;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;

namespace FilmShelf.BAL.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> AddReviewAsync(ReviewAddDTO reviewAddDTO)
    {
        var newReview = new Review
        {
            UserId = reviewAddDTO.UserId,
            MovieId = reviewAddDTO.MovieId,
            Content = reviewAddDTO.Content,
            Rating = reviewAddDTO.Rating,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ReviewRepository
            .AddReviewAsync(newReview);
        await _unitOfWork.SaveAsync();

        return newReview.Id;
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        await _unitOfWork.ReviewRepository
            .DeleteReviewAsync(reviewId);
        await _unitOfWork.SaveAsync();
    }

    public async Task<ReviewDTO?> GetReviewByIdAsync(int reviewId)
    {
        var review = await _unitOfWork.ReviewRepository
            .GetReviewByIdAsync(reviewId);

        if (review == null)
        {
            return null;
        }

        return review.ToReviewDTO();
    }

    public async Task<List<ReviewDTO>> GetReviewsByMovieIdAsync(int movieId)
    {
        var reviews = await _unitOfWork.ReviewRepository
            .GetReviewsByMovieIdAsync(movieId);

        return reviews
            .Select(r => r.ToReviewDTO())
            .ToList();
    }

    public async Task<List<ReviewDTO>> GetReviewsByUserIdAsync(int userId)
    {
        var reviews = await _unitOfWork.ReviewRepository
            .GetReviewsByUserIdAsync(userId);

        return reviews
            .Select(r => r.ToReviewDTO())
            .ToList();
    }

    public async Task UpdateReviewAsync(int reviewId, string content, byte rating)
    {
        await _unitOfWork.ReviewRepository
            .UpdateReviewAsync(reviewId, content, rating);
        await _unitOfWork.SaveAsync();
    }

    public async Task<int> AddReviewResponseAsync(ReviewResponseAddDTO reviewResponseAddDTO)
    {
        var reviewResponse = new ReviewResponse
        {
            ReviewId = reviewResponseAddDTO.ReviewId,
            UserId = reviewResponseAddDTO.UserId,
            Content = reviewResponseAddDTO.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ReviewRepository
            .AddReviewResponseAsync(reviewResponse);
        await _unitOfWork.SaveAsync();

        return reviewResponse.Id;
    }

    public async Task<List<ReviewResponseDTO>> GetReviewResponsesByReviewIdAsync(int reviewId)
    {
        var reviewResponses = await _unitOfWork.ReviewRepository
            .GetResponsesByReviewIdAsync(reviewId);

        return reviewResponses
            .Select(r => r.ToReviewResponseDTO())
            .ToList();
    }

    public async Task<ReviewResponseDTO?> GetReviewResponseByIdAsync(int reviewId)
    {
        var reviewResponse = await _unitOfWork.ReviewRepository
            .GetResponseByIdAsync(reviewId);

        if (reviewResponse == null)
        {
            return null;
        }

        return reviewResponse.ToReviewResponseDTO();
    }

    public async Task DeleteReviewResponseAsync(int reviewResponseId, int userId)
    {
        await _unitOfWork.ReviewRepository
            .DeleteReviewResponseAsync(reviewResponseId, userId);
        await _unitOfWork.SaveAsync();
    }
}
