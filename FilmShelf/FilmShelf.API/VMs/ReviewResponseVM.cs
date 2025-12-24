namespace FilmShelf.API.VMs;

public class ReviewResponseVM
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = null!;
}
