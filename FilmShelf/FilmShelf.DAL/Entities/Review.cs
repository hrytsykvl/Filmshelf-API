using FilmShelf.DAL.Identity;
using System.ComponentModel.DataAnnotations;

namespace FilmShelf.DAL.Entities;

public class Review
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int MovieId { get; set; }
    
    [Range(0, 10)]
    public byte Rating { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}
