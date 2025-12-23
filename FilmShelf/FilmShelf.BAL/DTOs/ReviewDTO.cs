namespace FilmShelf.BAL.DTOs;

public class ReviewDTO
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public byte Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserName { get; set; } = null!;
    public string MovieTitle { get; set; } = null!;
    public List<ReviewResponseDTO> Responses { get; set; } = new();
}
