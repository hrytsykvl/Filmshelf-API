using FilmShelf.BAL.DTOs;
using Google.Apis.Auth;

namespace FilmShelf.BAL.Interfaces;

public interface IAccountService
{
    Task<AuthenticationResponseDTO> RegisterUserAsync(RegisterDTO registerDTO);
    Task<bool> IsEmailInUseAsync(string email);
    Task<AuthenticationResponseDTO> LoginUserAsync(LoginDTO loginDTO);
    Task<AuthenticationResponseDTO> GenerateNewAccessTokenAsync(TokenModelDTO tokenModel);
    Task<PasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
    Task<PasswordResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
    Task LogoutUserAsync(string refreshToken);
    Task<AuthenticationResponseDTO?> AuthenticateGoogleUserAsync(string googleIdToken);
}
