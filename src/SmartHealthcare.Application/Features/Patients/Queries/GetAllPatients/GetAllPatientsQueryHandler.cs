using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetAllPatients;

public class GetAllPatientsQueryHandler : IQueryHandler<GetAllPatientsQuery, IReadOnlyList<PatientResponse>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public GetAllPatientsQueryHandler(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<PatientResponse>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PatientResponse>>(patients);
    }
}
