using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler : ICommandHandler<CreatePatientCommand, PatientResponse>
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePatientCommandHandler(
        IPatientRepository patientRepository,
        IUserRepository userRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _patientRepository = patientRepository;
        _userRepository = userRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var existing = await _patientRepository.GetByNationalCodeAsync(request.NationalCode, cancellationToken);
        if (existing is not null)
        {
            throw new DomainException($"Patient with national code '{request.NationalCode}' already exists.");
        }

        var (succeeded, identityUserId, errors) = await _identityService.CreateIdentityUserAsync(request.Email, request.Password, cancellationToken);
        if (!succeeded)
        {
            var errorMsg = string.Join(", ", errors);
            throw new DomainException($"Failed to create user account: {errorMsg}");
        }

        try
        {
            var user = new User(request.FirstName, request.LastName, request.Email, UserRole.Patient, identityUserId);
            await _userRepository.AddAsync(user, cancellationToken);

            var patient = new Patient(
                user.Id,
                request.NationalCode,
                request.DateOfBirth,
                request.Gender,
                request.PhoneNumber,
                request.MedicalInsuranceNumber,
                request.BloodGroup
            );

            await _patientRepository.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PatientResponse>(patient);
        }
        catch
        {
            await _identityService.DeleteIdentityUserAsync(identityUserId, cancellationToken);
            throw;
        }
    }
}
