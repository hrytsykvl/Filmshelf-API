namespace FilmShelf.BAL.DTOs;

public class ForgotPasswordDTO
{
    public string Email { get; set; } = null!;
    public string ResetPasswordUrl { get; set; } = null!;
}
