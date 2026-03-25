using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IRecommendationService
{
    Task<List<MovieDTO>> RecommendForUser(int userId, int? holdOutMovieId = null);
}
