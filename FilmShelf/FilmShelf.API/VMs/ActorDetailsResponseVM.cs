namespace FilmShelf.API.VMs;

public class ActorDetailsResponseVM
{
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Bio { get; set; } = null!;
    public string ProfilePath { get; set; } = null!;
    public List<ActorMoviesVM> Movies { get; set; } = new ();
}
