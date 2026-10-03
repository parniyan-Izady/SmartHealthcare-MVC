using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Appointments.Commands.BookAppointment;

public class BookAppointmentCommandHandler : ICommandHandler<BookAppointmentCommand, AppointmentResponse>
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public BookAppointmentCommandHandler(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AppointmentResponse> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException("Patient", request.PatientId);
        }

        var currentUserId = _currentUserService.UserId;
        var isStaff = _currentUserService.IsInRole(UserRole.Admin, UserRole.Receptionist);

        if (!isStaff && patient.IdentityUserId != currentUserId)
        {
            throw new ForbiddenAccessException("You do not have permission to book appointments for another patient.");
        }

        var doctor = await _doctorRepository.GetWithDetailsAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            throw new NotFoundException("Doctor", request.DoctorId);
        }

        // Check for overlapping appointments
        var existingAppointments = await _appointmentRepository.GetDoctorAppointmentsForDateAsync(
            request.DoctorId, request.StartTimeUtc, cancellationToken);

        bool isOverlapping = existingAppointments.Any(a =>
            a.AppointmentStartUtc < request.EndTimeUtc && request.StartTimeUtc < a.AppointmentEndUtc);

        if (isOverlapping)
        {
            throw new DoctorScheduleConflictException(request.DoctorId, request.StartTimeUtc, request.EndTimeUtc);
        }

        var appointment = _mapper.Map<Appointment>(request);

        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Fetch detailed appointment for mapping patient and doctor details
        var detailedAppointment = await _appointmentRepository.GetWithDetailsAsync(appointment.Id, cancellationToken) ?? appointment;

        return _mapper.Map<AppointmentResponse>(detailedAppointment);
    }
}
