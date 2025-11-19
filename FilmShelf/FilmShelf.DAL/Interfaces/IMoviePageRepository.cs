using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IMoviePageRepository  
{
    Task<MoviePage?> GetPageAsync(int pageNumber);
    Task AddPageAsync(MoviePage page);
    void UpdatePage(MoviePage pageToUpdate, MoviePage page);
}
