using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ReviewResponseAddVMValidator : AbstractValidator<ReviewResponseAddVM>
{
    public ReviewResponseAddVMValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Content must not be empty and must be less than 1000 characters");
    }
}
