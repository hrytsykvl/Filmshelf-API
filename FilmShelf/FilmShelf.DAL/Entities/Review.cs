namespace FilmShelf.DAL.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }


    public Movie Movie { get; set; } = null!;
}
