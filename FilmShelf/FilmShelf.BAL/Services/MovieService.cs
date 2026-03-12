using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MappingExtensions;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.Options;
using FilmShelf.TMDbClient.Responses;
using Microsoft.Extensions.Options;
using AutoMapper;
using System.Text.Json;

namespace FilmShelf.BAL.Services;

public class MovieService : IMovieService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieApiIntegrationService _movieApiIntegrationService;
    private readonly TmdbSettings _tmdbSettings;
    private readonly IMapper _mapper;

    public MovieService(
        IUnitOfWork unitOfWork,
        IMovieApiIntegrationService movieApiIntegrationService,
        IOptions<TmdbSettings> tmdbSettings,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _movieApiIntegrationService = movieApiIntegrationService;
        _tmdbSettings = tmdbSettings.Value;
        _mapper = mapper;
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
                AverageRating = movie.AverageRating,
                Cast = movie.MovieActors
                    .Select(ma => new CastMemberDTO
                    {
                        Id = ma.Actor.Id,
                        Name = ma.Actor.Name,
                        Character = ma.Role,
                        ProfilePath = ma.Actor.ProfilePath
                    })
                    .ToList()
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

        var directorDetails = await _movieApiIntegrationService
            .FetchPersonDetailsAsync<DirectorDetailsResponse>(directorId);
        if (directorDetails == null)
        {
            return null;
        }

        await AddMovieWithDetails(movieId, movieDetails, directorId, directorDetails, movieCredits);

        var movieDetailsDTO = movieDetails.ToMovieDetailsDTO(
            movieCredits.Cast.Take(_tmdbSettings.NumberOfActors),
            directorDetails.Name);

        return movieDetailsDTO;
    }

    public async Task<List<PopularMovieDTO>> GetPopularMoviesAsync()
    {
        var (jsonResponse, popularMovies) = await _movieApiIntegrationService
            .FetchPopularMoviesAsync(_tmdbSettings.PopularMoviesToFetch);

        var popularMoviesDTO = popularMovies
            .Select(pm => new PopularMovieDTO
            {
                Id = pm.Id,
                Title = pm.Title,
                PosterPath = PhotoPathGenerator.GeneratePosterPath(pm.PosterPath),
                AverageRating = pm.AverageRating,
                ReleaseDate = pm.ReleaseDate
            })
            .ToList();

        return popularMoviesDTO;
    }

    public async Task<BulkImportResultDTO> BulkImportFromPagesAsync()
    {
        var pages = await _unitOfWork.MoviePageRepository.GetAllPagesAsync();

        var tmdbIds = new HashSet<int>();

        foreach (var page in pages)
        {
            using var doc = JsonDocument.Parse(page.MoviesJson);

            if (!doc.RootElement.TryGetProperty("results", out var results))
                continue;

            foreach (var movie in results.EnumerateArray())
            {
                if (movie.TryGetProperty("id", out var idElement)
                    && idElement.TryGetInt32(out var id))
                {
                    tmdbIds.Add(id);
                }
            }
        }

        return await BulkImportFromTmdbAsync(tmdbIds.ToList());
    }

    public async Task<BulkImportResultDTO> BulkImportFromTmdbAsync(List<int> tmdbIds)
    {
        var result = new BulkImportResultDTO();

        foreach (var movieId in tmdbIds)
        {
            var existing = await _unitOfWork.MovieRepository.GetMovieAsync(movieId);
            if (existing != null)
            {
                result.Skipped++;
                continue;
            }

            try
            {
                var movieDetails = await _movieApiIntegrationService.FetchMovieDetailsAsync(movieId);
                if (movieDetails == null)
                {
                    result.Failed++;
                    result.FailedIds.Add(movieId);
                    continue;
                }

                var movieCredits = await _movieApiIntegrationService.FetchMovieCreditsAsync(movieId);
                if (movieCredits?.Crew == null)
                {
                    result.Failed++;
                    result.FailedIds.Add(movieId);
                    continue;
                }

                var directorCrew = movieCredits.Crew
                    .FirstOrDefault(c => c.Job == _tmdbSettings.CrewJob);
                if (directorCrew == null)
                {
                    result.Failed++;
                    result.FailedIds.Add(movieId);
                    continue;
                }

                var directorDetails = await _movieApiIntegrationService
                    .FetchPersonDetailsAsync<DirectorDetailsResponse>(directorCrew.Id);
                if (directorDetails == null)
                {
                    result.Failed++;
                    result.FailedIds.Add(movieId);
                    continue;
                }

                await AddMovieWithDetails(movieId, movieDetails, directorCrew.Id, directorDetails, movieCredits);
                result.Saved++;
            }
            catch (Exception)
            {
                result.Failed++;
                result.FailedIds.Add(movieId);
            }
        }

        return result;
    }

    public async Task<List<MovieDTO>> SearchMovie(string searchQuery)
    {
        var movies = await _movieApiIntegrationService
            .SearchMovie(searchQuery);

        var movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

        return movieDTOs;
    }

    private async Task AddMovieWithDetails(
        int movieId,
        MovieDetailsResponse movieDetails,
        int directorId,
        DirectorDetailsResponse directorDetails,
        MovieCreditsResponse movieCredits)
    {
        await AddDirector(directorId, directorDetails);
        await AddMovie(movieId, movieDetails, directorId);
        await AddGenres(movieId, movieDetails);
        await AddActorsAndRoles(movieId, movieCredits);

        await _unitOfWork.SaveAsync();
    }

    private async Task AddDirector(int directorId, DirectorDetailsResponse directorDetails)
    {
        var existingDirector = await _unitOfWork.DirectorRepository.GetDirectorAsync(directorId);
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
    }

    private async Task AddMovie(int movieId, MovieDetailsResponse movieDetails, int directorId)
    {
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
    }

    private async Task AddGenres(int movieId, MovieDetailsResponse movieDetails)
    {
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
    }

    private async Task AddActorsAndRoles(int movieId, MovieCreditsResponse movieCredits)
    {
        var fetchedActors = movieCredits.Cast.Take(_tmdbSettings.NumberOfActors);
        var actorsWithRoles = fetchedActors
            .Select(actor => (ActorId: actor.Id, Role: actor.Character))
            .ToList();

        var existingActors = await _unitOfWork.ActorRepository
            .GetActorsAsync(actorsWithRoles.Select(a => a.ActorId).ToList());

        var missingActors = fetchedActors
            .Where(actor => !existingActors.Any(existing => existing.Id == actor.Id))
            .Select(actor => new Actor
            {
                Id = actor.Id,
                Name = actor.Name,
                ProfilePath = PhotoPathGenerator.GeneratePosterPath(actor.ProfilePath)
            })
            .ToList();

        if (missingActors.Any())
        {
            await _unitOfWork.ActorRepository.AddActorsAsync(missingActors);
        }

        await _unitOfWork.MovieRepository.AddMovieActorsAsync(movieId, actorsWithRoles);
    }
}
