using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class ActorRequestVMValidator : AbstractValidator<ActorRequestVM>
{
    public ActorRequestVMValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Actor id must be greater than 0");
    }
}
