namespace FilmShelf.BAL.DTOs;

public class ReviewResponseAddDTO
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
}
