using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
