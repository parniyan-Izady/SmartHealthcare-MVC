using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetPatientReports;

public class GetPatientReportsQueryHandler : IQueryHandler<GetPatientReportsQuery, IReadOnlyList<PatientReportDto>>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientReportsQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IReadOnlyList<PatientReportDto>> Handle(GetPatientReportsQuery request, CancellationToken cancellationToken)
    {
        return await _patientRepository.GetPatientReportsAsync(
            request.SearchTerm, 
            request.DoctorId, 
            cancellationToken);
    }
}
