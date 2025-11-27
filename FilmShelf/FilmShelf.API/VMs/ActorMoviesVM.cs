namespace FilmShelf.API.VMs;

public class ActorMoviesVM
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public string Role { get; set; } = null!;
}
