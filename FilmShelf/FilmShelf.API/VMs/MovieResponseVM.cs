namespace FilmShelf.API.VMs;

public class MovieResponseVM
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
}
