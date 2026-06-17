using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class MovieRequestVM
{
    [FromRoute(Name = "id")]
    public int Id { get; set; }

    [FromQuery(Name = "language")]
    public string Language { get; set; } = "en-US";
}
