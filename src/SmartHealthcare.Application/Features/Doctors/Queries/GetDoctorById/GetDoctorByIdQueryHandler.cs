using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryHandler : IQueryHandler<GetDoctorByIdQuery, DoctorResponse?>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public GetDoctorByIdQueryHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<DoctorResponse?> Handle(GetDoctorByIdQuery request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return null;
        }

        return _mapper.Map<DoctorResponse>(doctor);
    }
}
