namespace SmartHealthcare.Application.DTOs;

public record AuthResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Role,
    string Token
);
