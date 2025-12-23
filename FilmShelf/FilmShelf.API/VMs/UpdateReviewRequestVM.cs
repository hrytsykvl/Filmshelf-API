using System.ComponentModel.DataAnnotations;

namespace FilmShelf.API.VMs;

public class UpdateReviewRequestVM
{
    public string Content { get; set; } = null!;
    public byte Rating { get; set; }
}
