using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class WatchlistAddVMValidator : AbstractValidator<WatchlistAddVM>
{
    public WatchlistAddVMValidator()
    {
        RuleFor(x => x.WatchlistId)
            .GreaterThan(0)
            .WithMessage("Watchlist id must be greater than 0");

        RuleFor(x => x.MovieId)
            .GreaterThan(0)
            .WithMessage("Movie id must be greater than 0");
    }
}
