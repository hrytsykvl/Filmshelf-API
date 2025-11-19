using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MappingExtensions;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.Options;
using Microsoft.Extensions.Options;

namespace FilmShelf.BAL.Services;

public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieApiIntegrationService _movieApiIntegrationService;
    private readonly TmdbSettings _tmdbSettings;

    public MovieService(
        IUnitOfWork unitOfWork,
        IMovieApiIntegrationService movieApiIntegrationService,
        IOptions<TmdbSettings> tmdbSettings)
    {
        _unitOfWork = unitOfWork;
        _movieApiIntegrationService = movieApiIntegrationService;
        _tmdbSettings = tmdbSettings.Value;
    }

    public async Task<MovieDetailsDTO?> GetMovieAsync(int movieId)
    {
        var movie = await _unitOfWork.MovieRepository.GetMovieAsync(movieId);

        if (movie != null)
        {
            return new MovieDetailsDTO
            {
                Title = movie.Title,
                Director = movie.Director.Name,
                Genres = movie.MovieGenres
                    .Select(mg => new GenreDTO
                    {
                        Id = mg.Genre.Id,
                        Name = mg.Genre.Name
                    })
                    .ToList(),
                Overview = movie.Overview,
                ReleaseDate = movie.ReleaseDate,
                Runtime = movie.Runtime,
                PosterPath = movie.PosterPath,
                AverageRating = movie.AverageRating
            };
        }

        var movieDetails = await _movieApiIntegrationService.FetchMovieDetailsAsync(movieId);
        if (movieDetails == null)
        {
            return null;
        }

        var movieCredits = await _movieApiIntegrationService.FetchMovieCreditsAsync(movieId);
        if (movieCredits == null
            || movieCredits.Crew == null)
        {
            return null;
        }

        var directorId = movieCredits.Crew
                .First(c => c.Job == _tmdbSettings.CrewJob).Id;

        var directorDetails = await _movieApiIntegrationService.FetchDirectorDetailsAsync(directorId);
        if (directorDetails == null)
        {
            return null;
        }

        var existingDirector = await _unitOfWork.DirectorRepository.GetDirectorAsync(directorId);

        await _unitOfWork.CreateTransactionAsync();
        if (existingDirector == null)
        {
            var newDirector = new Director
            {
                Id = directorId,
                Name = directorDetails.Name,
                Bio = directorDetails.Biography,
                BirthDate = directorDetails.Birthday
            };
            await _unitOfWork.DirectorRepository.AddDirectorAsync(newDirector);
        }

        var newMovie = new Movie
        {
            Id = movieId,
            Title = movieDetails.Title,
            Overview = movieDetails.Overview,
            ReleaseDate = movieDetails.ReleaseDate,
            DirectorId = directorId,
            Runtime = movieDetails.Runtime,
            PosterPath = PhotoPathGenerator.GeneratePosterPath(movieDetails.PosterPath),
            AverageRating = movieDetails.AverageRating
        };
        await _unitOfWork.MovieRepository.AddMovieAsync(newMovie);

        var genreIds = movieDetails.Genres.Select(genre => genre.Id).ToList();
        var existingGenres = await _unitOfWork.GenreRepository.GetGenresAsync(genreIds);

        var missingGenres = movieDetails.Genres
            .Where(genre => !existingGenres.Any(existing => existing.Id == genre.Id))
            .Select(genre => new Genre { Id = genre.Id, Name = genre.Name })
            .ToList();
        
        if (missingGenres.Any())
        {
            await _unitOfWork.GenreRepository.AddGenresAsync(missingGenres);
        }

        await _unitOfWork.MovieRepository.AddMovieGenresAsync(movieId, genreIds);
        await _unitOfWork.SaveAsync();
        await _unitOfWork.CommitTransactionAsync();

        var movieDetailsDTO = movieDetails.ToMovieDetailsDTO(
            movieCredits.Cast.Take(_tmdbSettings.NumberOfActors),
            directorDetails.Name);

        return movieDetailsDTO;
    }
}
