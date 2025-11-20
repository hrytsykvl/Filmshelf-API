using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class PageRequestVM
{
    [FromQuery(Name = "page")]
    public int Page { get; set; }
}
