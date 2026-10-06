using FluentValidation;

namespace SmartHealthcare.Application.Features.Appointments.Commands.MarkNoShow;

public class MarkNoShowCommandValidator : AbstractValidator<MarkNoShowCommand>
{
    public MarkNoShowCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required.");
    }
}
