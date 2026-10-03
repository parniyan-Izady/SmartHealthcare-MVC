using AutoMapper;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Features.Doctors.Commands.CreateDoctor;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Mappings;

public class DoctorMappingProfile : Profile
{
    public DoctorMappingProfile()
    {
        CreateMap<Doctor, DoctorResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.IdentityUserId))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));

        CreateMap<CreateDoctorCommand, Doctor>()
            .ConstructUsing((src, ctx) => new Doctor(
                ctx.Items.ContainsKey("UserId") ? (Guid)ctx.Items["UserId"] : Guid.Empty,
                src.FirstName,
                src.LastName,
                src.Email,
                src.PhoneNumber,
                src.MedicalLicenseNumber,
                src.MedicalSpecialty,
                src.ConsultationFee,
                src.OfficeAddress
            ));
    }
}
