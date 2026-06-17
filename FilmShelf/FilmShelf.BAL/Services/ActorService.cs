using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.BAL.MappingExtensions;
using FilmShelf.DAL.Interfaces;
using FilmShelf.TMDbClient.Interfaces;
using FilmShelf.TMDbClient.Options;
using FilmShelf.TMDbClient.Responses;

namespace FilmShelf.BAL.Services;

public class ActorService : IActorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMovieApiIntegrationService _movieApiIntegrationService;

    public ActorService(
        IUnitOfWork unitOfWork,
        IMovieApiIntegrationService movieApiIntegrationService)
    {
        _unitOfWork = unitOfWork;
        _movieApiIntegrationService = movieApiIntegrationService;
    }

    public async Task<ActorDetailsDTO?> GetActorDetailsAsync(int actorId, string language = LanguageConstants.English)
    {
        var actor = await _unitOfWork.ActorRepository.GetActorAsync(actorId);

        if (actor == null)
        {
            return null;
        }

        if (language == LanguageConstants.English && actor.Bio != null && actor.BirthDate.HasValue)
        {
            return actor.ToActorDetailsDTO();
        }

        var actorDetails = await _movieApiIntegrationService
            .FetchPersonDetailsAsync<ActorDetailsResponse>(actorId, language);
        if (actorDetails == null)
        {
            return null;
        }

        if (language == LanguageConstants.English)
        {
            _unitOfWork.ActorRepository.UpdateActor(
                actor,
                actorDetails.Biography,
                actorDetails.Birthday);

            await _unitOfWork.SaveAsync();
        }

        return actor.ToActorDetailsDTOWithOverride(actorDetails.Biography, actorDetails.Birthday);
    }
}
