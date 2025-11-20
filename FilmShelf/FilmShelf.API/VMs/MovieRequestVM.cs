using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class MovieRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }
}
