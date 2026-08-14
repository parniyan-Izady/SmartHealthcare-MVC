using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetAllPatients;

public record GetAllPatientsQuery : IQuery<IReadOnlyList<PatientResponse>>;
