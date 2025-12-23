using FilmShelf.DAL.Data;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FilmShelf.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FilmsDbContext _context;

    public IDirectorRepository DirectorRepository { get; private set; }
    public IGenreRepository GenreRepository { get; private set; }
    public IMovieRepository MovieRepository { get; private set; }
    public IActorRepository ActorRepository { get; private set; }
    public IMoviePageRepository MoviePageRepository { get; private set; }
    public IWatchlistRepository WatchlistRepository { get; private set; }
    public IReviewRepository ReviewRepository { get; private set; }

    public UnitOfWork(FilmsDbContext context)
    {
        _context = context;
        DirectorRepository = new DirectorRepository(_context);
        GenreRepository = new GenreRepository(_context);
        MovieRepository = new MovieRepository(_context);
        ActorRepository = new ActorRepository(_context);
        MoviePageRepository = new MoviePageRepository(_context);
        WatchlistRepository = new WatchlistRepository(_context);
        ReviewRepository = new ReviewRepository(_context);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
