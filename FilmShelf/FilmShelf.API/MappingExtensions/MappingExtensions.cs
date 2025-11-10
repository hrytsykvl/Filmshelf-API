using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;

namespace FilmShelf.API.MappingExtensions;
public static class MappingExtensions
{
    public static RegisterDTO ToRegisterDTO(this RegisterVM registerVM)
    {
        return new RegisterDTO
        {
            PersonName = registerVM.PersonName,
            Email = registerVM.Email,
            Password = registerVM.Password,
            ConfirmationPassword = registerVM.ConfirmationPassword
        };
    }

    public static LoginDTO ToLoginDTO(this LoginVM loginVM)
    {
        return new LoginDTO
        {
            Email = loginVM.Email,
            Password = loginVM.Password
        };
    }

    public static TokenModelDTO ToTokenModelDTO(this TokenVM tokenVM)
    {
        return new TokenModelDTO
        {
            Token = tokenVM.Token,
            RefreshToken = tokenVM.RefreshToken
        };
    }

    public static LogoutDTO ToLogoutDTO(this LogoutVM logoutVM)
    {
        return new LogoutDTO
        {
            RefreshToken = logoutVM.RefreshToken
        };
    }

    public static ForgotPasswordDTO ToForgotPasswordDTO(this ForgotPasswordVM forgotPasswordVM)
    {
        return new ForgotPasswordDTO
        {
            Email = forgotPasswordVM.Email,
            ResetPasswordUrl = forgotPasswordVM.ResetPasswordUrl
        };
    }

    public static ResetPasswordDTO ToResetPasswordDTO(this ResetPasswordVM resetPasswordVM)
    {
        return new ResetPasswordDTO
        {
            Email = resetPasswordVM.Email,
            Token = resetPasswordVM.Token,
            Password = resetPasswordVM.Password
        };
    }

    public static AuthenticationResponseVM ToAuthenticationResponseVM (
        this AuthenticationResponseDTO authenticationResponseDTO) {
        return new AuthenticationResponseVM
        {
            Token = authenticationResponseDTO.Token,
            RefreshToken = authenticationResponseDTO.RefreshToken,
            RefreshTokenExpirationDate = authenticationResponseDTO.RefreshTokenExpirationDate
        };
    }

    public static PasswordResponseVM ToPasswordResponseVM(this PasswordResponseDTO passwordResponseDTO)
    {
        return new PasswordResponseVM
        {
            Message = passwordResponseDTO.Message
        };
    }
}
