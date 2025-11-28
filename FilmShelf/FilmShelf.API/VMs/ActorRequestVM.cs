using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class ActorRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }
}
