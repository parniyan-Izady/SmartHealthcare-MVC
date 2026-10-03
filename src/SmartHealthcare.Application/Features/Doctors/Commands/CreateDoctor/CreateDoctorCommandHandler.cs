using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler : ICommandHandler<CreateDoctorCommand, DoctorResponse>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _doctorRepository = doctorRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<DoctorResponse> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsInRole(UserRole.Admin))
        {
            throw new ForbiddenAccessException("Only administrators can register doctor profiles.");
        }
        var existingDoctor = await _doctorRepository.GetByLicenseNumberAsync(request.MedicalLicenseNumber, cancellationToken);
        if (existingDoctor is not null)
        {
            throw new DuplicateMedicalLicenseException(request.MedicalLicenseNumber);
        }

        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            UserRole.Doctor,
            cancellationToken);

        if (!succeeded)
        {
            var errorMsg = string.Join(", ", errors);
            throw new DomainException($"Failed to create doctor user account: {errorMsg}");
        }

        var doctor = _mapper.Map<Doctor>(request, opt => opt.Items["UserId"] = userId);
        await _doctorRepository.AddAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailedDoctor = await _doctorRepository.GetWithDetailsAsync(doctor.Id, cancellationToken) ?? doctor;

        return _mapper.Map<DoctorResponse>(detailedDoctor);
    }
}
