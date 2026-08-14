using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;

namespace SmartHealthcare.Application.Features.Appointments.Queries.GetPagedAppointments;

public class GetPagedAppointmentsQueryHandler : IQueryHandler<GetPagedAppointmentsQuery, PagedResult<AppointmentResponse>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public GetPagedAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AppointmentResponse>> Handle(GetPagedAppointmentsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.Page > 0 ? request.Page : 1;
        int size = request.PageSize > 0 ? request.PageSize : 10;

        var (items, totalCount) = await _appointmentRepository.GetPagedAppointmentsAsync(
            request.DoctorId,
            request.PatientId,
            request.Status,
            request.FromDateUtc,
            request.ToDateUtc,
            pageNumber,
            size,
            request.SortBy ?? "StartUtc",
            request.SortOrder ?? "asc",
            cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<AppointmentResponse>>(items);
        return new PagedResult<AppointmentResponse>(dtos, totalCount, pageNumber, size);
    }
}
