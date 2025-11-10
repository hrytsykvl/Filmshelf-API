namespace FilmShelf.DAL.Entities;

public class Director
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string Bio { get; set; } = null!;


    public List<Movie> Movies { get; set; } = new List<Movie>();
}
