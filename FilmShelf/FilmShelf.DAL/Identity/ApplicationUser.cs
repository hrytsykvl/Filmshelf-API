using FilmShelf.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace FilmShelf.DAL.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public List<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
    public List<Review> Reviews { get; set; } = new List<Review>();
}
