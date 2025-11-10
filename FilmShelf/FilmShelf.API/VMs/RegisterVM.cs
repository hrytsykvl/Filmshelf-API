namespace FilmShelf.API.VMs;

public class RegisterVM
{
    public string PersonName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmationPassword { get; set; } = null!;
}
