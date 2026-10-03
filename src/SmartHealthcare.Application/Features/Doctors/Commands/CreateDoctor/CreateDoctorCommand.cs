using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Doctors.Commands.CreateDoctor;

public record CreateDoctorCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string MedicalLicenseNumber,
    string MedicalSpecialty,
    decimal ConsultationFee,
    string OfficeAddress
) : ICommand<DoctorResponse>;
