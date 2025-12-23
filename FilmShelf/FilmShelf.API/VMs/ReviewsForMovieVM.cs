using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.VMs;

public class ReviewsForMovieVM
{
    [FromRoute(Name = "movieId")]
    public int MovieId { get; set; }
}
