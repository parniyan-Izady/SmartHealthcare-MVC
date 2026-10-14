using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Staff.Commands.DeleteStaff;

public record DeleteStaffCommand(Guid Id) : ICommand;
