using FilmShelf.DAL.Data;
using FilmShelf.DAL.Entities;
using FilmShelf.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FilmShelf.DAL.Repositories;

public class ActorRepository : IActorRepository
{
    private readonly FilmsDbContext _context;

    public ActorRepository(FilmsDbContext context)
    {
        _context = context;
    }

    public async Task<Actor?> GetActorAsync(int actorId)
    {
        return await _context.Actors
            .Include(a => a.MovieActors)
            .ThenInclude(ma => ma.Movie)
            .SingleOrDefaultAsync(a => a.Id == actorId);
    }

    public async Task<List<Actor>> GetActorsAsync(List<int> actorIds)
    {
        return await _context.Actors
            .Where(a => actorIds.Contains(a.Id))
            .ToListAsync();
    }

    public async Task AddActorsAsync(List<Actor> actors)
    {
        await _context.Actors.AddRangeAsync(actors);
    }

    public void UpdateActor(Actor actor, string bio, DateTime birthDate)
    {
        actor.Bio = bio;
        actor.BirthDate = birthDate;
    }
}
