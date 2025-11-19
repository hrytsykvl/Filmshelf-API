using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class MovieRequestVMValidator : AbstractValidator<MovieRequestVM>
{
    public MovieRequestVMValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
}
