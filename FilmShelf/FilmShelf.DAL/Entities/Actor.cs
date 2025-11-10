namespace FilmShelf.DAL.Entities;

public class Actor
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Bio { get; set; } = null!;

    public List<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
}
