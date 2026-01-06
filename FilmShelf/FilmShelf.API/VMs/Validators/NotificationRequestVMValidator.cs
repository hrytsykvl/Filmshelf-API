using FluentValidation;

namespace FilmShelf.API.VMs.Validators;

public class NotificationRequestVMValidator : AbstractValidator<NotificationRequestVM>
{
    public NotificationRequestVMValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Notification id must be greater than 0.");
    }
}
