namespace FilmShelf.DAL.Interfaces;

public interface IEvaluationRepository
{
    Task<List<(int UserId, int MovieId, byte Rating, DateTime CreatedAt)>> GetAllReviewsAsync();
    Task<List<int>> GetUserIdsWithMinReviewsAsync(int minReviews);
    Task<int> GetTotalMovieCountAsync();
}
