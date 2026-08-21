namespace SmartHealthcare.Application.DTOs;

public record AppointmentResponse(
    Guid Id,
    Guid PatientId,
    string PatientName,
    Guid DoctorId,
    string DoctorName,
    DateTime StartUtc,
    DateTime EndUtc,
    string Status,
    string? ReasonForVisit,
    string? CancellationReason
);
