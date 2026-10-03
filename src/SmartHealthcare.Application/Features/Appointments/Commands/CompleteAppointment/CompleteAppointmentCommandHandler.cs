using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Appointments.Commands.CompleteAppointment;

public class CompleteAppointmentCommandHandler : ICommandHandler<CompleteAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CompleteAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return false;
        }

        var isAdmin = _currentUserService.IsInRole(UserRole.Admin);
        var isAssignedDoctor = appointment.DoctorId == _currentUserService.DoctorId;

        if (!isAdmin && !isAssignedDoctor)
        {
            throw new ForbiddenAccessException("Only the attending doctor or an administrator can mark an appointment as completed.");
        }

        appointment.Complete();
        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
