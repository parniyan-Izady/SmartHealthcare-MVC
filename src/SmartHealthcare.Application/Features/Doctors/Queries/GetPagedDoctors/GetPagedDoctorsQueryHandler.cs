using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Doctors.Queries.GetPagedDoctors;

public class GetPagedDoctorsQueryHandler : IQueryHandler<GetPagedDoctorsQuery, PagedResult<DoctorResponse>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public GetPagedDoctorsQueryHandler(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<DoctorResponse>> Handle(GetPagedDoctorsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.Page > 0 ? request.Page : 1;
        int size = request.PageSize > 0 ? request.PageSize : 10;

        var (items, totalCount) = await _doctorRepository.GetPagedDoctorsAsync(
            request.Specialty,
            request.SearchTerm,
            request.IsActive,
            pageNumber,
            size,
            request.SortBy ?? "LastName",
            request.SortOrder ?? "asc",
            cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<DoctorResponse>>(items);
        return new PagedResult<DoctorResponse>(dtos, totalCount, pageNumber, size);
    }
}
