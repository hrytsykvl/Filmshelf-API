using FilmShelf.DAL.Entities;

namespace FilmShelf.DAL.Interfaces;

public interface IActorRepository
{
    Task<Actor?> GetActorAsync(int actorId);
    Task<List<Actor>> GetActorsAsync(List<int> actorIds);
    Task AddActorsAsync(List<Actor> actors);
    void UpdateActor(Actor actor, string bio, DateTime birthDate);
}
