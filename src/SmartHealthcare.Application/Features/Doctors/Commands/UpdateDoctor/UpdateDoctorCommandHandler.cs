using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler : ICommandHandler<UpdateDoctorCommand, DoctorResponse?>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<DoctorResponse?> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return null;
        }

        var isAdmin = _currentUserService.IsInRole(UserRole.Admin);

        if (!isAdmin && doctor.Id != _currentUserService.DoctorId)
        {
            throw new ForbiddenAccessException("You do not have permission to update this doctor's profile.");
        }

        // به‌روزرسانی مشخصات فردی در دامین خود Doctor
        doctor.UpdateProfile(request.FirstName, request.LastName, doctor.Email, doctor.PhoneNumber);

        // به‌روزرسانی اطلاعات تخصصی و مطب
        doctor.UpdateDetails(request.MedicalSpecialty, request.ConsultationFee, request.OfficeAddress);

        _doctorRepository.Update(doctor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DoctorResponse>(doctor);
    }
}
