using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Appointments.Commands.MarkNoShow;

public record MarkNoShowCommand(Guid Id) : ICommand<bool>;
