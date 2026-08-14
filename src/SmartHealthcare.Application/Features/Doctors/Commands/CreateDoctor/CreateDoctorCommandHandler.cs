using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
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
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        IIdentityService identityService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _identityService = identityService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DoctorResponse> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var existingDoctor = await _doctorRepository.GetByLicenseNumberAsync(request.MedicalLicenseNumber, cancellationToken);
        if (existingDoctor is not null)
        {
            throw new DomainException("Doctor with this medical license number already exists.");
        }

        var (succeeded, identityUserId, errors) = await _identityService.CreateIdentityUserAsync(request.Email, request.Password, cancellationToken);
        if (!succeeded)
        {
            var errorMsg = string.Join(", ", errors);
            throw new DomainException($"Failed to create user account: {errorMsg}");
        }

        try
        {
            var user = new User(request.FirstName, request.LastName, request.Email, UserRole.Doctor, identityUserId);
            _dbContext.Users.Add(user);

            var doctor = new Doctor(user.Id, request.MedicalLicenseNumber, request.MedicalSpecialty, request.ConsultationFee, request.OfficeAddress);
            await _doctorRepository.AddAsync(doctor, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<DoctorResponse>(doctor);
        }
        catch
        {
            await _identityService.DeleteIdentityUserAsync(identityUserId, cancellationToken);
            throw;
        }
    }
}
