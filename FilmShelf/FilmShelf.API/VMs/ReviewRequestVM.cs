using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class ReviewRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }
}
