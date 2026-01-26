using FilmShelf.API.MappingExtensions;
using FilmShelf.API.VMs;
using FilmShelf.BAL.Helpers;
using FilmShelf.BAL.Interfaces;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FilmShelf.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;
    private readonly IReviewService _reviewService;

    public AccountController(IAccountService accountService,
        IReviewService reviewService,
        ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthenticationResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterVM registerVM)
    {
        var registerDTO = registerVM.ToRegisterDTO();
        var authenticationResponseDTO = await _accountService.RegisterUserAsync(registerDTO);

        var authenticationResponse = authenticationResponseDTO.ToAuthenticationResponseVM();
        return Ok(authenticationResponse);
    }

    [HttpGet("email-check")]
    [ProducesResponseType(typeof(EmailInUseVM), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsEmailInUse(string email)
    {
        var isEmailInUse = await _accountService.IsEmailInUseAsync(email);
        return Ok(new EmailInUseVM
        {
            Email = email,
            IsInUse = isEmailInUse
        });
    }

    [HttpPost("token")]
    [ProducesResponseType(typeof(AuthenticationResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
    {
        var loginDTO = loginVM.ToLoginDTO();
        var authenticationResponseDTO = await _accountService.LoginUserAsync(loginDTO);

        var authenticationResponse = authenticationResponseDTO.ToAuthenticationResponseVM();
        return Ok(authenticationResponse);
    }

    [HttpPost("google-login")]
    [ProducesResponseType(typeof(AuthenticationResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleTokenVM googleTokenVM)
    {
        var authenticationResponse = await _accountService.AuthenticateGoogleUserAsync(googleTokenVM.IdToken);

        if (authenticationResponse == null)
        {
            return Unauthorized();
        }

        return Ok(authenticationResponse);
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutVM logoutVM)
    {
        var refreshToken = logoutVM.ToLogoutDTO().RefreshToken;
        await _accountService.LogoutUserAsync(refreshToken);
        return NoContent();
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthenticationResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateNewAccessToken([FromBody] TokenVM tokenVM)
    {
        var tokenModel = tokenVM.ToTokenModelDTO();
        var authenticationResponseDTO = await _accountService.GenerateNewAccessTokenAsync(tokenModel);

        var authenticationResponse = authenticationResponseDTO.ToAuthenticationResponseVM();
        return Ok(authenticationResponse);
    }

    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(PasswordResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordVM forgotPasswordVM)
    {
        var forgotPasswordDTO = forgotPasswordVM.ToForgotPasswordDTO();
        var passwordResponse = await _accountService.ForgotPasswordAsync(forgotPasswordDTO);

        var passwordResponseVM = passwordResponse.ToPasswordResponseVM();
        return Ok(passwordResponseVM);
    }

    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(PasswordResponseVM), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordVM resetPasswordVM)
    {
        var resetPasswordDTO = resetPasswordVM.ToResetPasswordDTO();
        var passwordResponse = await _accountService.ResetPasswordAsync(resetPasswordDTO);

        var passwordResponseVM = passwordResponse.ToPasswordResponseVM();
        return Ok(passwordResponseVM);
    }

    [HttpGet("reviews")]
    [ProducesResponseType(typeof(IEnumerable<ReviewVM>), StatusCodes.Status200OK)]
    [Authorize]
    public async Task<IActionResult> RetrieveReviewsByUserId()
    {
        var userId = UserClaimsHelper.GetUserId(User);

        var reviewDTOs = await _reviewService
            .GetReviewsByUserIdAsync(userId);

        var reviewVMs = reviewDTOs
            .Select(r => r.ToReviewVM());

        return Ok(reviewVMs);
    }
}
