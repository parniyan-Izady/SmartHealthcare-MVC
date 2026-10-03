using AutoMapper;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<UserDto, AuthResponse>()
            .ForCtorParam("UserId", opt => opt.MapFrom(src => src.Id))
            .ForCtorParam("FullName", opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForCtorParam("Email", opt => opt.MapFrom(src => src.Email ?? string.Empty))
            .ForCtorParam("Role", opt => opt.MapFrom(src => src.Role.ToString()));
    }
}
