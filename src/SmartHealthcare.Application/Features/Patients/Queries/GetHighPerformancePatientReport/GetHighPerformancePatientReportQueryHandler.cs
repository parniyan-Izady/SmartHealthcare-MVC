using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetHighPerformancePatientReport;

/// <summary>
/// CQRS Read Model Query Handler executing optimized query via Repository.
/// </summary>
public class GetHighPerformancePatientReportQueryHandler : IQueryHandler<GetHighPerformancePatientReportQuery, IReadOnlyList<PatientReportDto>>
{
    private readonly IPatientRepository _patientRepository;

    public GetHighPerformancePatientReportQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IReadOnlyList<PatientReportDto>> Handle(GetHighPerformancePatientReportQuery request, CancellationToken cancellationToken)
    {
        return await _patientRepository.GetPatientReportsAsync(cancellationToken);
    }
}
