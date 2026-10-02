namespace SmartHealthcare.Application.DTOs;

public record PatientReportDto(
    Guid PatientId,
    string FullName,
    string NationalCode,
    string PhoneNumber,
    int TotalAppointmentsCount,
    DateTime? LastAppointmentDateUtc
);
