using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class RegisterVMValidator : AbstractValidator<RegisterVM>
{
    public RegisterVMValidator()
    {
        RuleFor(x => x.PersonName)
            .NotEmpty().WithMessage("Person Name can't be blank");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email can't be blank")
            .EmailAddress().WithMessage("Email should be in a valid email address format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password can't be blank");

        RuleFor(x => x.ConfirmationPassword)
            .NotEmpty().WithMessage("Confirmation Password can't be blank")
            .Equal(x => x.Password).WithMessage("Password and Confirmation Password do not match");
    }
}
