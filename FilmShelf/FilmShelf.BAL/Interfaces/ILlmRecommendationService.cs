using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface ILlmRecommendationService
{
    Task<List<LlmRecommendationDTO>> RecommendForUserAsync(int userId, int top = 10, int? holdOutMovieId = null, string language = "en-US");
}
