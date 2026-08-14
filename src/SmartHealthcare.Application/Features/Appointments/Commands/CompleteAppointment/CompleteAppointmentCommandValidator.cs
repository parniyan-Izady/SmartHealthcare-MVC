using FluentValidation;

namespace SmartHealthcare.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandValidator : AbstractValidator<CompleteAppointmentCommand>
{
    public CompleteAppointmentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required.");
    }
}
