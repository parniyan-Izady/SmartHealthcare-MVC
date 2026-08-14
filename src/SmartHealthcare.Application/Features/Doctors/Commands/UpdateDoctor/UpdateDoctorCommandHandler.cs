using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler : ICommandHandler<UpdateDoctorCommand, DoctorResponse?>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateDoctorCommandHandler(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DoctorResponse?> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetWithDetailsAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return null;
        }

        // Domain construction / entity property update
        typeof(Doctor).GetProperty(nameof(Doctor.MedicalSpecialty))?.SetValue(doctor, request.MedicalSpecialty);
        typeof(Doctor).GetProperty(nameof(Doctor.ConsultationFee))?.SetValue(doctor, request.ConsultationFee);
        typeof(Doctor).GetProperty(nameof(Doctor.OfficeAddress))?.SetValue(doctor, request.OfficeAddress);
        doctor.MarkUpdated();

        // Update User name if provided
        if (doctor.User is not null)
        {
            typeof(User).GetProperty(nameof(User.FirstName))?.SetValue(doctor.User, request.FirstName);
            typeof(User).GetProperty(nameof(User.LastName))?.SetValue(doctor.User, request.LastName);
            doctor.User.MarkUpdated();
        }

        _doctorRepository.Update(doctor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<DoctorResponse>(doctor);
    }
}
