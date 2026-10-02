using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserDto user);
}
