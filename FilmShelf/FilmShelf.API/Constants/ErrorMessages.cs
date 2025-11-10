namespace FilmShelf.API.Constants;

public static class ErrorMessages
{
    public const string InvalidEmail = "User registration was not successful due to invalid email.";
    public const string EmailInUse = "User registration was not successful due to email already in use.";
    public const string InvalidUsername = "User registration was not successful due to invalid username.";
    public const string InvalidPassword = "User registration was not successful due to invalid password.";
    public const string InvalidLogin = "Authentication failed, invalid email or password.";
    public const string InvalidToken = "Failed to generate new access token.";
    public const string InvalidRefreshToken = "Failed to generate new access token due to invalid refresh token.";
    public const string InvalidResetToken = "Reset password was not successful due to invalid reset token";
}
