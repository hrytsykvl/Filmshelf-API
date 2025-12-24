using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ReviewResponseRequestVMValidator : AbstractValidator<ReviewResponseRequestVM>
{
    public ReviewResponseRequestVMValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review response id must be greater than 0.");
    }
}
