using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface ICollaborativeRecommendationService
{
    Task<List<MovieDTO>> RecommendForUserAsync(int userId, int top = 10);
}
