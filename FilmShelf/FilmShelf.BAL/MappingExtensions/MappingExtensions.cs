using FilmShelf.BAL.DTOs;
using FilmShelf.DAL.Entities;
using FilmShelf.TMDbClient.Responses;

namespace FilmShelf.BAL.MappingExtensions;

public static class MappingExtensions
{
    public static MovieDetailsDTO ToMovieDetailsDTO(
        this MovieDetailsResponse movieDetailsResponse,
        IEnumerable<CastMemberResponse> castMembers,
        string directorName)
    {
        return new MovieDetailsDTO
        {
            Title = movieDetailsResponse.Title,
            Director = directorName,
            Genres = movieDetailsResponse.Genres
                .Select(g => new GenreDTO { Id = g.Id, Name = g.Name })
                .ToList(),
            Overview = movieDetailsResponse.Overview,
            ReleaseDate = movieDetailsResponse.ReleaseDate,
            Runtime = movieDetailsResponse.Runtime,
            PosterPath = movieDetailsResponse.PosterPath,
            AverageRating = movieDetailsResponse.AverageRating,
            Cast = castMembers
                .Select(cm => new CastMemberDTO
                {
                    Id = cm.Id,
                    Name = cm.Name,
                    Character = cm.Character,
                    ProfilePath = cm.ProfilePath
                })
                .ToList()
        };
    }

    public static ActorDetailsDTO ToActorDetailsDTO(
        this Actor actor)
    {
        return new ActorDetailsDTO
        {
            Name = actor.Name,
            Bio = actor.Bio!,
            BirthDate = actor.BirthDate!.Value,
            ProfilePath = actor.ProfilePath,
            Movies = actor.MovieActors
                .Select(ma => new ActorMoviesDTO
                {
                    Id = ma.MovieId,
                    Title = ma.Movie.Title,
                    PosterPath = ma.Movie.PosterPath,
                    Role = ma.Role
                })
                .ToList()
        };
    }
}
