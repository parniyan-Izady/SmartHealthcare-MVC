using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.DTOs;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string Token
);
