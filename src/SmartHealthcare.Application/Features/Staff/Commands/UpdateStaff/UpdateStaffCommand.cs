using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Staff.Commands.UpdateStaff;

public record UpdateStaffCommand(
    Guid Id,
    string Email,
    string PhoneNumber,
    UserRole Role
) : ICommand;
