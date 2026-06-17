using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class PageRequestVM
{
    [FromQuery(Name = "page")]
    public int? Page { get; set; }

    [FromQuery(Name = "filter")]
    public string? Filter { get; set; }

    [FromQuery(Name = "language")]
    public string Language { get; set; } = "en-US";
}
