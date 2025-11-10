using System.ComponentModel.DataAnnotations;

namespace FilmShelf.BAL.DTOs;

public class LoginDTO
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}
