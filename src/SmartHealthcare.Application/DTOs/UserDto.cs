using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.DTOs;

public record UserDto(
    Guid Id,
    string? Email,
    UserRole Role,
    bool IsActive
);
