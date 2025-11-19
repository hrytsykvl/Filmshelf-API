using FilmShelf.API.VMs;
using FilmShelf.BAL.DTOs;
using FilmShelf.BAL.Helpers;

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

    public static MovieListResponseVM ToMovieListResponseVM(
        this IEnumerable<MovieDTO> movies,
        int totalPages)
    {
        var moviesVM = movies.Select(m => m.ToMovieResponseVM()).ToList();

        return new MovieListResponseVM
        {
            MovieList = moviesVM,
            TotalPages = totalPages
        };
    }

    public static MovieDetailsResponseVM ToMovieDetailsResponseVM(
        this MovieDetailsDTO movieDetailsDTO)
    {
        return new MovieDetailsResponseVM
        {
            Title = movieDetailsDTO.Title,
            Director = movieDetailsDTO.Director,
            Genres = movieDetailsDTO.Genres.Select(g => g.Name).ToList(),
            Overview = movieDetailsDTO.Overview,
            ReleaseDate = movieDetailsDTO.ReleaseDate,
            Runtime = movieDetailsDTO.Runtime,
            PosterPath = PhotoPathGenerator.GeneratePosterPath(movieDetailsDTO.PosterPath),
            AverageRating = movieDetailsDTO.AverageRating,
            Cast = movieDetailsDTO.Cast.Select(c => c.ToCastMemberVM()).ToList()
        };
    }

    private static CastMemberVM ToCastMemberVM(
        this CastMemberDTO castMemberDTO)
    {
        return new CastMemberVM
        {
            Id = castMemberDTO.Id,
            Name = castMemberDTO.Name,
            Character = castMemberDTO.Character,
            ProfilePath = PhotoPathGenerator.GeneratePosterPath(castMemberDTO.ProfilePath)
        };
    }

    private static MovieResponseVM ToMovieResponseVM(
        this MovieDTO movieDTO)
    {
        return new MovieResponseVM
        {
            Id = movieDTO.Id,
            Title = movieDTO.Title,
            PosterPath = PhotoPathGenerator.GeneratePosterPath(movieDTO.PosterPath),
        };
    }
}
