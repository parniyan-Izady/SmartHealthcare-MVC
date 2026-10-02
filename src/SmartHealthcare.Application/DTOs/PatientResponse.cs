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
