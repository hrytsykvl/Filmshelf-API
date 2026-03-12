using FilmShelf.API.VMs;
using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class BulkImportRequestVMValidator : AbstractValidator<BulkImportRequestVM>
{
    private const int MaxIds = 100;

    public BulkImportRequestVMValidator()
    {
        RuleFor(x => x.TmdbIds)
            .NotEmpty()
            .WithMessage("At least one TMDB ID is required.")
            .Must(ids => ids.Count <= MaxIds)
            .WithMessage($"Cannot import more than {MaxIds} movies at once.");

        RuleForEach(x => x.TmdbIds)
            .GreaterThan(0)
            .WithMessage("All TMDB IDs must be positive integers.");
    }
}
