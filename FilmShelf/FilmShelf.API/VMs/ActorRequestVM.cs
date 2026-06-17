using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class ActorRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }

    [FromQuery(Name = "language")]
    public string Language { get; set; } = "en-US";
}
