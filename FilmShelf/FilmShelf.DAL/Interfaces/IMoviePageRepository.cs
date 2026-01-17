using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Enums;

namespace FilmShelf.DAL.Interfaces;

public interface IMoviePageRepository  
{
    Task<MoviePage?> GetPageAsync(
        int? pageNumber = null,
        MoviePageType? moviePageType = MoviePageType.Regular);
    Task AddPageAsync(MoviePage page);
    void UpdatePage(MoviePage pageToUpdate, MoviePage page);
}
