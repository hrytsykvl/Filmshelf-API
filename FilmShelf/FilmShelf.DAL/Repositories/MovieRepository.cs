using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly FilmsDbContext _context;

    public MovieRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<Movie?> GetMovieAsync(int movieId)
    {
        return await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.MovieGenres)
            .ThenInclude(mg => mg.Genre)
            .SingleOrDefaultAsync(m => m.Id == movieId);
    }

    public async Task AddMovieAsync(Movie movie)
    {
        await _context.Movies.AddAsync(movie);
    }

    public async Task AddMovieGenresAsync(int movieId, List<int> genreIds)
    {
        var movieGenres = genreIds.Select(genreId => new MovieGenre
        {
            MovieId = movieId,
            GenreId = genreId
        }).ToList();

        await _context.MovieGenres.AddRangeAsync(movieGenres);
    }
}
