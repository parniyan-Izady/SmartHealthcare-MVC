using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler : IQueryHandler<GetPatientByIdQuery, PatientResponse?>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetPatientByIdQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientResponse?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            return null;
        }

        return _mapper.Map<PatientResponse>(patient);
    }
}
