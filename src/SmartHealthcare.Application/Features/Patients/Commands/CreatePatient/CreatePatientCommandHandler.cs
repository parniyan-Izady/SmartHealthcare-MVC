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
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePatientCommandHandler(
        IPatientRepository patientRepository,
        IIdentityService identityService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _patientRepository = patientRepository;
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PatientResponse> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
    {
        var existing = await _patientRepository.GetByNationalCodeAsync(request.NationalCode, cancellationToken);
        if (existing is not null)
        {
            throw new DuplicateNationalCodeException(request.NationalCode);
        }

        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            UserRole.Patient,
            cancellationToken);

        if (!succeeded)
        {
            var errorMsg = string.Join(", ", errors);
            throw new DomainException($"Failed to create user account: {errorMsg}");
        }

        var patient = _mapper.Map<Patient>(request, opt => opt.Items["UserId"] = userId);
        await _patientRepository.AddAsync(patient, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var detailedPatient = await _patientRepository.GetWithDetailsAsync(patient.Id, cancellationToken) ?? patient;

        return _mapper.Map<PatientResponse>(detailedPatient);
    }
}
