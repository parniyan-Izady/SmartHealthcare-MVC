using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Doctors.Commands.DeleteDoctor;

public class DeleteDoctorCommandHandler : ICommandHandler<DeleteDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsInRole(UserRole.Admin))
        {
            throw new ForbiddenAccessException("Only administrators can delete doctor profiles.");
        }

        var doctor = await _doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return false;
        }

        doctor.MarkDeleted();
        _doctorRepository.Update(doctor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
