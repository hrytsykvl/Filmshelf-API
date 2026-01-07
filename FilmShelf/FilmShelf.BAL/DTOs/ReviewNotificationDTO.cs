namespace FilmShelf.BAL.DTOs;

public class ReviewNotificationDTO
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = null!;
    public string MoviePoster { get; set; } = null!;
    public ReviewResponseDTO ReviewResponse { get; set; } = null!;
}
