using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ForgotPasswordVMValidator : AbstractValidator<ForgotPasswordVM>
{
    public ForgotPasswordVMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.ResetPasswordUrl)
            .NotEmpty().WithMessage("Reset password URL is required.")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Invalid URL format.");
    }
}
