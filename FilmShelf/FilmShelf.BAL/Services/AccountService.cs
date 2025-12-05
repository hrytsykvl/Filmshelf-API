using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Interfaces;
using FilmShelf.DAL.Identity;
using FilmShelf.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using FilmShelf.BAL.Exceptions;
using FilmShelf.BAL.Options;
using Microsoft.Extensions.Options;

namespace FilmShelf.BAL.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IWatchlistService _watchlistService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly WatchlistSettings _watchlistSettings;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        ITokenService jwtService,
        IEmailService emailService,
        IWatchlistService watchlistService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<WatchlistSettings> watchlistSettings)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _watchlistService = watchlistService;
        _refreshTokenRepository = refreshTokenRepository;
        _emailService = emailService;
        _watchlistSettings = watchlistSettings.Value;
    }

    public async Task<AuthenticationResponseDTO> RegisterUserAsync(RegisterDTO registerDTO)
    {
        var emailExists = _userManager.Users.Any(u => u.Email == registerDTO.Email);
        if (emailExists)
        {
            throw new EmailInUseException();
        }

        var user = new ApplicationUser
        {
            UserName = registerDTO.PersonName,
            Email = registerDTO.Email
        };

        var createUserResult = await _userManager.CreateAsync(user, registerDTO.Password);
        if (!createUserResult.Succeeded)
        {
            var usernameErrors = createUserResult.Errors
                .Where(e => e.Code.Contains(nameof(user.UserName)))
                .ToList();

            var passwordErrors = createUserResult.Errors
                .Where(e => e.Code.Contains(nameof(registerDTO.Password)))
                .ToList();

            if (usernameErrors.Any())
            {
                throw new InvalidUsernameException(string.Join(' ',
                    usernameErrors.Select(e => e.Description).ToList()));
            }
            else if (passwordErrors.Any())
            {
                throw new InvalidPasswordException(string.Join(' ',
                    passwordErrors.Select(e => e.Description).ToList()));
            }
        }

        var authenticationResponse = _jwtService.CreateJwtToken(user);

        var newRefreshToken = new RefreshToken
        {
            Token = authenticationResponse.RefreshToken,
            ExpirationDate = authenticationResponse.RefreshTokenExpirationDate,
            UserId = user.Id
        };
        await _refreshTokenRepository.AddRefreshTokenAsync(newRefreshToken);

        await _watchlistService.CreateWatchlistAsync(
            user.Id,
            _watchlistSettings.DefaultName,
            true);

        return authenticationResponse;
    }

    public async Task<bool> IsEmailInUseAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<AuthenticationResponseDTO> LoginUserAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        if (user == null
            || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
        {
            throw new InvalidLoginException();
        }

        var authenticationResponse = _jwtService.CreateJwtToken(user);

        var newRefreshToken = new RefreshToken
        {
            Token = authenticationResponse.RefreshToken,
            ExpirationDate = authenticationResponse.RefreshTokenExpirationDate,
            UserId = user.Id
        };
        await _refreshTokenRepository.AddRefreshTokenAsync(newRefreshToken);

        return authenticationResponse;
    }

    public async Task<AuthenticationResponseDTO> GenerateNewAccessTokenAsync(TokenModelDTO tokenModel)
    {
        ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(tokenModel.Token);

        if (principal == null)
        {
            throw new InvalidTokenException();
        }

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            throw new InvalidTokenException();
        }

        var existingToken = await _refreshTokenRepository.GetRefreshTokenAsync(tokenModel.RefreshToken);

        if (existingToken == null
            || existingToken.ExpirationDate <= DateTime.Now)
        {
            throw new InvalidRefreshTokenException();
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidTokenException();
        }

        var authenticationResponse = _jwtService.CreateJwtToken(user);

        await _refreshTokenRepository.UpdateRefreshTokenAsync(
            existingToken,
            authenticationResponse.RefreshToken,
            authenticationResponse.RefreshTokenExpirationDate);

        return authenticationResponse;
    }

    public async Task<PasswordResponseDTO> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);

        if (user == null)
        {
            throw new InvalidEmailException();
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"{forgotPasswordDTO.ResetPasswordUrl}" +
                        $"?token={Uri.EscapeDataString(resetToken)}" +
                        $"&email={Uri.EscapeDataString(forgotPasswordDTO.Email)}";

        await _emailService.SendPasswordResetEmailAsync(forgotPasswordDTO.Email, resetLink);

        var passwordResponse = new PasswordResponseDTO
        {
            Message = "Password reset email has been sent."
        };

        return passwordResponse;
    }

    public async Task<PasswordResponseDTO> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);

        if (user == null)
        {
            throw new InvalidEmailException();
        }

        var resetResult = await _userManager.ResetPasswordAsync(
            user,
            resetPasswordDTO.Token,
            resetPasswordDTO.Password);

        if (!resetResult.Succeeded)
        {
            var tokenErrors = resetResult.Errors
                .Where(e => e.Code.Contains(nameof(resetPasswordDTO.Token)))
                .ToList();

            var passwordErrors = resetResult.Errors
                .Where(e => e.Code.Contains(nameof(resetPasswordDTO.Password)))
                .ToList();

            if (tokenErrors.Any())
            {
                throw new InvalidResetTokenException(string.Join(' ',
                    tokenErrors.Select(e => e.Description).ToList()));
            }
            else if (passwordErrors.Any())
            {
                throw new InvalidPasswordException(string.Join(' ',
                    passwordErrors.Select(e => e.Description).ToList()));
            }
        }

        var passwordResponse = new PasswordResponseDTO
        {
            Message = "Password has been reset successfully."
        };

        return passwordResponse;
    }

    public async Task LogoutUserAsync(string refreshToken)
    {
        var existingToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

        if (existingToken != null)
        {
            await _refreshTokenRepository.RemoveRefreshTokenAsync(existingToken);
        }
    }
}
