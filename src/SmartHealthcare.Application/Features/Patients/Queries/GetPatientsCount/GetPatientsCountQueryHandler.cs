using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetPatientsCount;

public class GetPatientsCountQueryHandler : IQueryHandler<GetPatientsCountQuery, int>
{
    private readonly IPatientRepository _patientRepository;

    public GetPatientsCountQueryHandler(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<int> Handle(GetPatientsCountQuery request, CancellationToken cancellationToken)
    {
        return await _patientRepository.CountAsync(cancellationToken: cancellationToken);
    }
}
