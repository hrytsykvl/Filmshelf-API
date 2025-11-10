namespace FilmShelf.API.VMs;

public class ForgotPasswordVM
{
    public string Email { get; set; } = null!;
    public string ResetPasswordUrl { get; set; } = null!;
}
