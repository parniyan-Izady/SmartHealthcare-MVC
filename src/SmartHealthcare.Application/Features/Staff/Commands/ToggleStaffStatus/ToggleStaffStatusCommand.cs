using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Staff.Commands.ToggleStaffStatus;

public record ToggleStaffStatusCommand(Guid Id) : ICommand<bool>;
