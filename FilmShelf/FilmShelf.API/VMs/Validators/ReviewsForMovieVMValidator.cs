using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ReviewsForMovieVMValidator : AbstractValidator<ReviewsForMovieVM>
{
    public ReviewsForMovieVMValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Movie id must be greater than 0");
    }
}
