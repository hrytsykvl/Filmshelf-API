using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface ILlamaRecommendationService
{
    Task<List<LlmRecommendationDTO>> RecommendForUserAsync(int userId, int top = 10, int? holdOutMovieId = null);
}
