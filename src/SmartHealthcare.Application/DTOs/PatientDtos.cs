namespace SmartHealthcare.Application.DTOs;

public record PatientResponse(
    Guid Id,
    Guid UserId,
    string FullName,
    string Email,
    string NationalCode,
    DateTime DateOfBirth,
    string Gender,
    string PhoneNumber,
    string? MedicalInsuranceNumber,
    string? BloodGroup
);

public record PatientReportDto(
    Guid PatientId,
    string FullName,
    string NationalCode,
    string PhoneNumber,
    int TotalAppointmentsCount,
    DateTime? LastAppointmentDateUtc
);
