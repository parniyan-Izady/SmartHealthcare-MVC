using AutoMapper;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // 1. Patient Mappings
        CreateMap<Patient, PatientResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : "Unknown"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()));

        // 2. Doctor Mappings
        CreateMap<Doctor, DoctorResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : "Unknown"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User != null && src.User.IsActive));

        // 3. Appointment Mappings
        CreateMap<Appointment, AppointmentResponse>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? $"{src.Patient.User.FirstName} {src.Patient.User.LastName}" : "Unknown Patient"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.User != null ? $"Dr. {src.Doctor.User.FirstName} {src.Doctor.User.LastName}" : "Unknown Doctor"))
            .ForMember(dest => dest.StartUtc, opt => opt.MapFrom(src => src.AppointmentStartUtc))
            .ForMember(dest => dest.EndUtc, opt => opt.MapFrom(src => src.AppointmentEndUtc))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        // 4. MedicalRecord Mappings
        CreateMap<MedicalRecord, MedicalRecordResponse>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? $"{src.Patient.User.FirstName} {src.Patient.User.LastName}" : "Unknown Patient"))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null && src.Doctor.User != null ? $"Dr. {src.Doctor.User.FirstName} {src.Doctor.User.LastName}" : "Unknown Doctor"))
            .ForMember(dest => dest.CreatedAtUtc, opt => opt.MapFrom(src => src.CreatedAtUtc));
    }
}
