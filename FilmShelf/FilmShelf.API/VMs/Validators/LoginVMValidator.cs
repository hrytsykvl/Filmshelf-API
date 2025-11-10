using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class LoginVMValidator : AbstractValidator<LoginVM>
{
    public LoginVMValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email can't be blank")
            .EmailAddress().WithMessage("Email should be in a valid email address format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password can't be blank");
    }
}
