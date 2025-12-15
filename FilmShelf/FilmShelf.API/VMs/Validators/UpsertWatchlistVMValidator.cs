using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class UpsertWatchlistVMValidator : AbstractValidator<UpsertWatchlistVM>
{
    public UpsertWatchlistVMValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title must not exceed 100 characters.");
    }
}
