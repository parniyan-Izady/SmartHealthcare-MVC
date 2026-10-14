using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Staff.Commands.CreateStaff;

public record CreateStaffCommand(
    string Email,
    string PhoneNumber,
    string Password,
    UserRole Role
) : ICommand<Guid>;
