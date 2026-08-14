using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetHighPerformancePatientReport;

public record GetHighPerformancePatientReportQuery : IQuery<IReadOnlyList<PatientReportDto>>;
