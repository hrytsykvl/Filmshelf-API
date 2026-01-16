using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Enums;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class MoviePageRepository : IMoviePageRepository
{
    private readonly FilmsDbContext _context;

    public MoviePageRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<MoviePage?> GetPageAsync(
        int? pageNumber = null,
        MoviePageType? moviePageType = MoviePageType.Regular)
    {
        var pageToFind = pageNumber ?? 1;

        return await _context.MoviePages
            .FirstOrDefaultAsync(p => p.PageNumber == pageToFind
                                     && p.Type == moviePageType);
    }

    public async Task AddPageAsync(MoviePage page)
    {
        await _context.MoviePages.AddAsync(page);
    }

    public void UpdatePage(MoviePage pageToUpdate, MoviePage page)
    {
        pageToUpdate.MoviesJson = page.MoviesJson;
        pageToUpdate.UpdatedAt = page.UpdatedAt;
    }
}
