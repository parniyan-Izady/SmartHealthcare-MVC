using FluentValidation;

namespace SmartHealthcare.Application.Features.Staff.Commands.ToggleStaffStatus;

public class ToggleStaffStatusCommandValidator : AbstractValidator<ToggleStaffStatusCommand>
{
    public ToggleStaffStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Staff member ID is required.");
    }
}
