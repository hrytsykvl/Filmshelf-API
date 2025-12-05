using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class WatchlistRequestVMValidator : AbstractValidator<WatchlistRequestVM>
{
    public WatchlistRequestVMValidator()
    {
        RuleFor(x => x.WatchlistId)
            .GreaterThan(0)
            .WithMessage("Watchlist id must be greater than 0");
    }
}
