using AutoMapper;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Features.Appointments.Commands.BookAppointment;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Mappings;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<Appointment, AppointmentResponse>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? $"{src.Patient.FirstName} {src.Patient.LastName}" : "Unknown Patient"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? $"{src.Doctor.FirstName} {src.Doctor.LastName}" : "Unknown Doctor"))
            .ForMember(dest => dest.StartUtc, opt => opt.MapFrom(src => src.AppointmentStartUtc))
            .ForMember(dest => dest.EndUtc, opt => opt.MapFrom(src => src.AppointmentEndUtc))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<BookAppointmentCommand, Appointment>()
            .ConstructUsing(src => new Appointment(
                src.PatientId,
                src.DoctorId,
                src.StartTimeUtc,
                src.EndTimeUtc,
                src.ReasonForVisit
            ));
    }
}
