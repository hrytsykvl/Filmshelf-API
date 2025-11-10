using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FilmShelf.BAL.DTOs;

public class RegisterDTO
{
    public string PersonName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmationPassword { get; set; } =  null!;
}
