using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IMovieRepository
{
    Task<Movie?> GetMovieAsync(int movieId);
    Task AddMovieAsync(Movie movie);
    Task AddMovieGenresAsync(int movieId, List<int> genreIds);
    Task AddMovieActorsAsync(int movieId, List<(int ActorId, string Role)> actorsWithRoles);
}
