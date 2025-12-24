namespace FilmShelf.BAL.DTOs;

public class ReviewAddDTO
{
    public string Content { get; set; } = null!;
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public byte Rating { get; set; }
}
