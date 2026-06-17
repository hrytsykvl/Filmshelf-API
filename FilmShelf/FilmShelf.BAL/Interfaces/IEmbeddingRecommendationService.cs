using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IEmbeddingRecommendationService
{
    Task<List<MovieDTO>> RecommendForUserAsync(int userId, int top = 10, int? holdOutMovieId = null, string language = "en-US");
}
