using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Appointments.Queries.GetAppointmentById;

public class GetAppointmentByIdQueryHandler : IQueryHandler<GetAppointmentByIdQuery, AppointmentResponse?>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetAppointmentByIdQueryHandler(
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AppointmentResponse?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return null;
        }

        var currentUserId = _currentUserService.UserId;
        var isStaff = _currentUserService.IsInRole(UserRole.Admin, UserRole.Receptionist, UserRole.Nurse);

        if (!isStaff)
        {
            var isDoctor = _currentUserService.IsInRole(UserRole.Doctor);
            var isPatient = _currentUserService.IsInRole(UserRole.Patient);

            bool isAuthorized = false;

            if (isDoctor && appointment.DoctorId == _currentUserService.DoctorId)
            {
                isAuthorized = true;
            }

            if (isPatient && appointment.PatientId == _currentUserService.PatientId)
            {
                isAuthorized = true;
            }

            if (!isAuthorized)
            {
                throw new ForbiddenAccessException("You do not have permission to view this appointment.");
            }
        }

        return _mapper.Map<AppointmentResponse>(appointment);
    }
}
