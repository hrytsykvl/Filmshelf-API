using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IDirectorRepository
{
    Task<Director?> GetDirectorAsync(int directorId);
    Task AddDirectorAsync(Director director);
}
