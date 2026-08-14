using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Doctors.Queries.SearchDoctorsBySpecialty;

public record SearchDoctorsBySpecialtyQuery(string Specialty) : IQuery<IReadOnlyList<DoctorResponse>>;
