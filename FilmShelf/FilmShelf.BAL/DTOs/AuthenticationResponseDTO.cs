namespace FilmShelf.BAL.DTOs;

public class AuthenticationResponseDTO
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpirationDate { get; set; }
}
