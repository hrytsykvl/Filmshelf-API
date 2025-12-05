using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class WatchlistActionVMValidator : AbstractValidator<WatchlistActionVM>
{
    public WatchlistActionVMValidator()
    {
        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Movie id must be greater than 0");
    }
}
