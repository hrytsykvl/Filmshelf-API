using Microsoft.AspNetCore.Identity;

namespace FilmShelf.DAL.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
