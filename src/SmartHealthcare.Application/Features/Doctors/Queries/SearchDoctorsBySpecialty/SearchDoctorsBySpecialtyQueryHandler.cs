using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Doctors.Queries.SearchDoctorsBySpecialty;

public class SearchDoctorsBySpecialtyQueryHandler : IQueryHandler<SearchDoctorsBySpecialtyQuery, IReadOnlyList<DoctorResponse>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public SearchDoctorsBySpecialtyQueryHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<DoctorResponse>> Handle(SearchDoctorsBySpecialtyQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Specialty))
        {
            return Array.Empty<DoctorResponse>();
        }

        var doctors = await _doctorRepository.SearchBySpecialtyAsync(request.Specialty, cancellationToken);
        return _mapper.Map<IReadOnlyList<DoctorResponse>>(doctors);
    }
}
