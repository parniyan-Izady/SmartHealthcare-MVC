using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.DTOs;

public record StaffMemberDto(
    Guid Id,
    string Email,
    string? PhoneNumber,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAtUtc
);

public record UpdateStaffMemberDto(
    Guid Id,
    string Email,
    string PhoneNumber,
    UserRole Role
);

