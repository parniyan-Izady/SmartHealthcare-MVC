using AutoMapper;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Features.Patients.Commands.CreatePatient;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Mappings;

public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Patient, PatientResponse>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()));

        CreateMap<Patient, PatientReportDto>()
            .ForCtorParam("PatientId", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("FullName", opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForCtorParam("NationalCode", opt => opt.MapFrom(src => src.NationalCode))
            .ForCtorParam("PhoneNumber", opt => opt.MapFrom(src => src.PhoneNumber))
            .ForCtorParam("TotalAppointmentsCount", opt => opt.MapFrom(src => src.Appointments.Count(a => !a.IsDeleted)))
            .ForCtorParam("LastAppointmentDateUtc", opt => opt.MapFrom(src => src.Appointments.Where(a => !a.IsDeleted).Max(a => (DateTime?)a.AppointmentStartUtc)));

        CreateMap<CreatePatientCommand, Patient>()
            .ConstructUsing((src, ctx) => new Patient(
                ctx.Items.ContainsKey("UserId") ? (Guid)ctx.Items["UserId"] : Guid.Empty,
                src.FirstName,
                src.LastName,
                src.Email,
                src.PhoneNumber,
                src.NationalCode,
                src.DateOfBirth,
                src.Gender,
                src.MedicalInsuranceNumber,
                src.BloodGroup
            ));
    }
}
