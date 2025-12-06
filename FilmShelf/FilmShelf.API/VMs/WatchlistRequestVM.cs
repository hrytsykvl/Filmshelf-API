using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class WatchlistRequestVM
{
    [FromRoute(Name = "id")]
    public int WatchlistId { get; set; }
}
