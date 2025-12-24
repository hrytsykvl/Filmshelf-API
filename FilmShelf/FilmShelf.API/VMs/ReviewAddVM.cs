namespace FilmShelf.API.VMs;

public class ReviewAddVM
{
    public string Content { get; set; } = null!;
    public int MovieId { get; set; }
    public byte Rating { get; set; }
}
