using FilmShelf.DAL.Identity;

namespace FilmShelf.DAL.Entities;

public class ReviewResponse
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Review Review { get; set; } = null!;
}
