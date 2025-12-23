using FilmShelf.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace FilmShelf.DAL.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public List<UserWatchlist> Watchlists { get; set; } = new List<UserWatchlist>();
    public List<Review> Reviews { get; set; } = new List<Review>();
    public List<ReviewResponse> ReviewResponses { get; set; } = new List<ReviewResponse>();
}
