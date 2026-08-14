using SmartHealthcare.Application.Common.CQRS;

namespace SmartHealthcare.Application.Features.Doctors.Commands.DeleteDoctor;

public record DeleteDoctorCommand(Guid Id) : ICommand<bool>;
