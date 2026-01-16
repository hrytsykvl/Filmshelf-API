using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class PageRequestVMValidator : AbstractValidator<PageRequestVM>
{
    public PageRequestVMValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .When(x => x.Page.HasValue)
            .WithMessage("Page number must be greater than 0");

        RuleFor(x => x.Filter)
            .MaximumLength(50)
            .When(x => x.Filter != null)
            .WithMessage("Filter must be less than 50 characters");
    }
}
