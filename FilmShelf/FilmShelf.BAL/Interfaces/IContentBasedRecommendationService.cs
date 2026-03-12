using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IContentBasedRecommendationService
{
    Task<List<MovieDTO>> RecommendForUserAsync(int userId, int top = 10);
}
