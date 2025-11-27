namespace FilmShelf.BAL.DTOs;

public class ActorDetailsDTO
{
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Bio { get; set; } = null!;
    public string ProfilePath { get; set; } = null!;
    public List<ActorMoviesDTO> Movies { get; set; } = new();
}
