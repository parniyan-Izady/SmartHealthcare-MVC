using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Appointments.Commands.CancelAppointment;

public class CancelAppointmentCommandHandler : ICommandHandler<CancelAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return false;
        }

        var isAdminOrStaff = _currentUserService.IsInRole(UserRole.Admin, UserRole.Receptionist);
        var isAssignedDoctor = appointment.DoctorId == _currentUserService.DoctorId;
        var isOwnerPatient = appointment.PatientId == _currentUserService.PatientId;

        if (!isAdminOrStaff && !isAssignedDoctor && !isOwnerPatient)
        {
            throw new ForbiddenAccessException("You do not have permission to cancel this appointment.");
        }

        appointment.Cancel(request.Reason);
        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
