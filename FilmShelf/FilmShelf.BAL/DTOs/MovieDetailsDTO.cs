namespace FilmShelf.BAL.DTOs;

public class MovieDetailsDTO
{
    public string Title { get; set; } = null!;
    public string Director { get; set; } = null!;
    public List<GenreDTO> Genres { get; set; } = new();
    public string Overview { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public short Runtime { get; set; }
    public string PosterPath { get; set; } = null!;
    public float AverageRating { get; set; }
    public List<CastMemberDTO> Cast { get; set; } = new();
}
