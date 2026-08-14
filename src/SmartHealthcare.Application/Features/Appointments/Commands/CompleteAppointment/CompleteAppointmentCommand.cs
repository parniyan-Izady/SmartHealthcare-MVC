using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand(Guid Id) : ICommand<bool>;
