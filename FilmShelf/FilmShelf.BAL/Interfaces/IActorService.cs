using FilmShelf.BAL.DTOs;

namespace FilmShelf.BAL.Interfaces;

public interface IActorService
{
    Task<ActorDetailsDTO?> GetActorDetailsAsync(int actorId);
}
