using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class WatchlistAddVM
{
    [FromRoute(Name = "watchlistId")]
    public int WatchlistId { get; set; }
    [FromRoute(Name = "movieId")]
    public int MovieId { get; set; }
}
