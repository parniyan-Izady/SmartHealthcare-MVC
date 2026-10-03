using FluentValidation;

namespace SmartHealthcare.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryValidator : AbstractValidator<GetAppointmentByIdQuery>
{
    public GetAppointmentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Appointment ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Appointment ID cannot be empty.");
    }
}
