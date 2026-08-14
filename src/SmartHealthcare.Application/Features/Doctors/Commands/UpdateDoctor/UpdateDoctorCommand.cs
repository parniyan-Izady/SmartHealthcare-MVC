using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Doctors.Commands.UpdateDoctor;

public record UpdateDoctorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string MedicalSpecialty,
    decimal ConsultationFee,
    string OfficeAddress
) : ICommand<DoctorResponse?>;
