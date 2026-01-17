namespace FilmShelf.API.VMs;

public class NotificationVM
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }

    public int? MovieId { get; set; }
    public string? MovieTitle { get; set; }
    public string? MoviePoster { get; set; }
    public ReviewResponseVM? ReviewResponse { get; set; }
}
