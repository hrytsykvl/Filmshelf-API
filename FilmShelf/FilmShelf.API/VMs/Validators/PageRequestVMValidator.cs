using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class PageRequestVMValidator : AbstractValidator<PageRequestVM>
{
    public PageRequestVMValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than 0");
    }
}
