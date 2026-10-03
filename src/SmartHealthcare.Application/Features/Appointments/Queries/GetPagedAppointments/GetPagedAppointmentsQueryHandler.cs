using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Appointments.Queries.GetPagedAppointments;

public class GetPagedAppointmentsQueryHandler : IQueryHandler<GetPagedAppointmentsQuery, PagedResult<AppointmentResponse>>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetPagedAppointmentsQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<AppointmentResponse>> Handle(GetPagedAppointmentsQuery request, CancellationToken cancellationToken)
    {
        int pageNumber = request.Page > 0 ? request.Page : 1;
        int size = request.PageSize > 0 ? request.PageSize : 10;

        Guid? effectiveDoctorId = request.DoctorId;
        Guid? effectivePatientId = request.PatientId;

        var isStaff = _currentUserService.IsInRole(UserRole.Admin, UserRole.Receptionist);

        if (!isStaff)
        {
            if (_currentUserService.IsInRole(UserRole.Doctor))
            {
                effectiveDoctorId = _currentUserService.DoctorId;
            }
            else if (_currentUserService.IsInRole(UserRole.Patient))
            {
                effectivePatientId = _currentUserService.PatientId;
            }
        }

        var (items, totalCount) = await _appointmentRepository.GetPagedAppointmentsAsync(
            effectiveDoctorId,
            effectivePatientId,
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
