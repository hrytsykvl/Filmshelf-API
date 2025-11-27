namespace FilmShelf.BAL.DTOs;

public class ActorMoviesDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public string Role { get; set; } = null!;
}
