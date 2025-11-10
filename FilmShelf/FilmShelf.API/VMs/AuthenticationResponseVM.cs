namespace FilmShelf.API.VMs;

public class AuthenticationResponseVM
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpirationDate { get; set; }
}
