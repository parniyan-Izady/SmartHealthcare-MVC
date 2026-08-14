using FluentValidation;

namespace SmartHealthcare.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommandValidator : AbstractValidator<BookAppointmentCommand>
{
    public BookAppointmentCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty().WithMessage("Patient ID is required.");

        RuleFor(x => x.DoctorId)
            .NotEmpty().WithMessage("Doctor ID is required.");

        RuleFor(x => x.StartTimeUtc)
            .NotEmpty().WithMessage("Start time is required.")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5)).WithMessage("Start time must be in the future.");

        RuleFor(x => x.EndTimeUtc)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTimeUtc).WithMessage("End time must be after start time.");

        RuleFor(x => x.ReasonForVisit)
            .MaximumLength(500).WithMessage("Reason for visit cannot exceed 500 characters.");
    }
}
