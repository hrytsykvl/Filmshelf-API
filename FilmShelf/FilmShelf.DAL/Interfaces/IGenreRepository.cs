using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IGenreRepository
{
    Task<List<Genre>> GetGenresAsync(List<int> genreIds);
    Task AddGenresAsync(List<Genre> genre);
}
