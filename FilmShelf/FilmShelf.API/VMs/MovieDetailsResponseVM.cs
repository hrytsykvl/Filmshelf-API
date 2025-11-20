namespace FilmShelf.API.VMs;

public class MovieDetailsResponseVM
{
    public string Title { get; set; } = null!;
    public string Director { get; set; } = null!;
    public List<string> Genres { get; set; } = new();
    public string Overview { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public short Runtime { get; set; }
    public string PosterPath { get; set; } = null!;
    public float AverageRating { get; set; }
    public List<CastMemberVM> Cast { get; set; } = new();
}
