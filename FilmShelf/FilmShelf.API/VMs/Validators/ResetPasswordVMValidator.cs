using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ResetPasswordVMValidator : AbstractValidator<ResetPasswordVM>
{
    public ResetPasswordVMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("New password is required.");
    }
}
