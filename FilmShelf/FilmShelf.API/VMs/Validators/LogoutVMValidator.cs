using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class LogoutVMValidator : AbstractValidator<LogoutVM>
{
    public LogoutVMValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token can't be blank");
    }
}
