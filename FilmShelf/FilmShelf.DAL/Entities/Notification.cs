using FilmShelf.DAL.Identity;

namespace FilmShelf.DAL.Entities;

public class Notification
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
