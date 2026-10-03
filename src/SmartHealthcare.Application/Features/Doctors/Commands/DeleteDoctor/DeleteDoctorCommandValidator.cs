using FluentValidation;

namespace SmartHealthcare.Application.Features.Doctors.Commands.DeleteDoctor;

public class DeleteDoctorCommandValidator : AbstractValidator<DeleteDoctorCommand>
{
    public DeleteDoctorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Doctor ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Doctor ID cannot be empty.");
    }
}
