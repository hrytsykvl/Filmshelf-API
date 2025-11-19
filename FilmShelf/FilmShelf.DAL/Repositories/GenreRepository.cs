using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly FilmsDbContext _context;

    public GenreRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Genre>> GetGenresAsync(List<int> genreIds)
    {
        return await _context.Genres
            .Where(g => genreIds.Contains(g.Id))
            .ToListAsync();
    }

    public async Task AddGenresAsync(List<Genre> genres)
    {
        await _context.Genres.AddRangeAsync(genres);
    }
}
