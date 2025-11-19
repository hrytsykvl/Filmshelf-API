namespace FilmShelf.DAL.Interfaces;

public interface IUnitOfWork
{
    IDirectorRepository DirectorRepository {  get; }
    IGenreRepository GenreRepository { get; }
    IMovieRepository MovieRepository { get; }
    IMoviePageRepository MoviePageRepository { get; }
    Task CreateTransactionAsync();
    Task CommitTransactionAsync();
    Task SaveAsync();
}
