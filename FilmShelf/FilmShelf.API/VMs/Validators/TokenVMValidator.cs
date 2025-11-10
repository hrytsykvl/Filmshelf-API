using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class TokenVMValidator : AbstractValidator<TokenVM>
{
    public TokenVMValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Jwt token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh Token is required");
    }
}