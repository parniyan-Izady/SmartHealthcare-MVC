using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Appointments.Commands.MarkNoShow;

public class MarkNoShowCommandHandler : ICommandHandler<MarkNoShowCommand, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public MarkNoShowCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(MarkNoShowCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return false;
        }

        var isAdminOrStaff = _currentUserService.IsInRole(UserRole.Admin, UserRole.Receptionist);
        var isAssignedDoctor = appointment.DoctorId == _currentUserService.DoctorId;

        if (!isAdminOrStaff && !isAssignedDoctor)
        {
            throw new ForbiddenAccessException("Only staff or the attending doctor can mark an appointment as No-Show.");
        }

        appointment.MarkNoShow();
        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
