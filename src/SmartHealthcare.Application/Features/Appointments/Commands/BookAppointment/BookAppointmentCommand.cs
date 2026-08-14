using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Appointments.Commands.BookAppointment;

public record BookAppointmentCommand(
    Guid PatientId,
    Guid DoctorId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    string? ReasonForVisit
) : ICommand<AppointmentResponse>;
