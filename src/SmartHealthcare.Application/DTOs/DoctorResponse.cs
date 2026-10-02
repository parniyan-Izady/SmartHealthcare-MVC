namespace SmartHealthcare.Application.DTOs;

public record DoctorResponse(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string MedicalLicenseNumber,
    string MedicalSpecialty,
    decimal ConsultationFee,
    string OfficeAddress,
    bool IsActive
);
