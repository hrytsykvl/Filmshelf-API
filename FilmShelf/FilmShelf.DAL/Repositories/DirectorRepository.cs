using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class DirectorRepository : IDirectorRepository
{
    private readonly FilmsDbContext _context;

    public DirectorRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<Director?> GetDirectorAsync(int directorId)
    {
        return await _context.Directors
            .SingleOrDefaultAsync(m => m.Id == directorId);
    }

    public async Task AddDirectorAsync(Director director)
    {
        await _context.Directors.AddAsync(director);
    }
}
