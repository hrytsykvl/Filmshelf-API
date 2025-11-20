namespace FilmShelf.DAL.Entities;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
