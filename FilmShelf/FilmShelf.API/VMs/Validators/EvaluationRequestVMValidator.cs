using FilmShelf.API.VMs;
using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class EvaluationRequestVMValidator : AbstractValidator<EvaluationRequestVM>
{
    public EvaluationRequestVMValidator()
    {
        RuleFor(x => x.K)
            .InclusiveBetween(1, 50)
            .WithMessage("K must be between 1 and 50.");

        RuleFor(x => x.MinReviews)
            .InclusiveBetween(1, 100)
            .WithMessage("MinReviews must be between 1 and 100.");

        RuleFor(x => x.RelevanceThreshold)
            .InclusiveBetween((byte)0, (byte)10)
            .WithMessage("RelevanceThreshold must be between 0 and 10.");
    }
}
