using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class CreateWatchlistVMValidator : AbstractValidator<CreateWatchlistVM>
{
    public CreateWatchlistVMValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(100)
            .WithMessage("Title must not exceed 100 characters.");
    }
}
