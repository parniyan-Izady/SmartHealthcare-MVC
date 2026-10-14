using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Staff.Commands.ResetStaffPassword;

public record ResetStaffPasswordCommand(
    Guid Id,
    string NewPassword
) : ICommand;
