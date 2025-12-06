namespace FilmShelf.DAL.Interfaces;

public interface IUnitOfWork
{
    IDirectorRepository DirectorRepository {  get; }
    IGenreRepository GenreRepository { get; }
    IMovieRepository MovieRepository { get; }
    IActorRepository ActorRepository { get; }
    IMoviePageRepository MoviePageRepository { get; }
    IWatchlistRepository WatchlistRepository { get; }
    Task SaveAsync();
}
