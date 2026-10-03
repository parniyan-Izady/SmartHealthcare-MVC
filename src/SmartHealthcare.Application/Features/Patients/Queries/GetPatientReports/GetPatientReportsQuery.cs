using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetPatientReports;

public record GetPatientReportsQuery(
    string? SearchTerm = null,
    Guid? DoctorId = null
) : IQuery<IReadOnlyList<PatientReportDto>>;
