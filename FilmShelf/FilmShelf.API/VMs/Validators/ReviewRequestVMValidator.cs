using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ReviewRequestVMValidator : AbstractValidator<ReviewRequestVM>
{
    public ReviewRequestVMValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review id must be greater than 0");
    }
}
