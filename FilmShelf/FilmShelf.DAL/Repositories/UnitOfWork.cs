using FilmShelf.DAL.Data;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FilmShelf.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FilmsDbContext _context;
    private IDbContextTransaction? _transaction;

    public IDirectorRepository DirectorRepository { get; private set; }

    public IGenreRepository GenreRepository { get; private set; }

    public IMovieRepository MovieRepository { get; private set; }
    public IMoviePageRepository MoviePageRepository { get; private set; }

    public UnitOfWork(FilmsDbContext context)
    {
        _context = context;
        DirectorRepository = new DirectorRepository(_context);
        GenreRepository = new GenreRepository(_context);
        MovieRepository = new MovieRepository(_context);
        MoviePageRepository = new MoviePageRepository(_context);
    }

    public async Task CreateTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
        {
            throw new ArgumentNullException("Transaction has not been started.");
        }

        await _transaction.CommitAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
