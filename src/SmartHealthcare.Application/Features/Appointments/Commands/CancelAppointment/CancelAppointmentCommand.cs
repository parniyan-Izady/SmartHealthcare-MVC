using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Appointments.Commands.CancelAppointment;

public record CancelAppointmentCommand(Guid Id, string Reason) : ICommand<bool>;
