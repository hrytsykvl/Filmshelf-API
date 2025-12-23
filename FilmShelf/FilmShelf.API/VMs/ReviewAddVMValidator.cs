using FluentValidation;

namespace FilmShelf.API.VMs;

public class ReviewAddVMValidator : AbstractValidator<ReviewAddVM>
{
    public ReviewAddVMValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000)
            .WithMessage("Content must be less than 1000 characters.");

        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Movie id must be greater than 0.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((byte)1, (byte)10)
            .WithMessage("Rating must be between 1 and 10.");
    }
}
