using FilmShelf.BAL.DTOs;
using FilmShelf.DAL.Identity;
using System.Security.Claims;

namespace FilmShelf.BAL.Interfaces;
public interface ITokenService
{
    ClaimsPrincipal? GetPrincipalFromJwtToken(string token);
    AuthenticationResponseDTO CreateJwtToken(ApplicationUser user);
}
