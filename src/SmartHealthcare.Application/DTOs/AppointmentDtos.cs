namespace SmartHealthcare.Application.DTOs;

public record BookAppointmentRequest(
    Guid PatientId,
    Guid DoctorId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    string? ReasonForVisit
);

public record CancelAppointmentRequest(
    string Reason
);

public record CompleteAppointmentRequest(
    string? Notes
);

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

public record AppointmentFilterRequest(
    Guid? DoctorId = null,
    Guid? PatientId = null,
    string? Status = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = "StartUtc",
    string? SortOrder = "asc"
);
