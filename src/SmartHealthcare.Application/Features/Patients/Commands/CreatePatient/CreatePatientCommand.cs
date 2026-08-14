using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Patients.Commands.CreatePatient;

public record CreatePatientCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string NationalCode,
    DateTime DateOfBirth,
    Gender Gender,
    string PhoneNumber,
    string? MedicalInsuranceNumber,
    string? BloodGroup
) : ICommand<PatientResponse>;
