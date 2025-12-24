using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class ReviewResponseRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }
}
