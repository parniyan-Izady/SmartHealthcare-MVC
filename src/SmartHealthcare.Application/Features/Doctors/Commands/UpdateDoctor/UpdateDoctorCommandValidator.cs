using FluentValidation;

namespace SmartHealthcare.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandValidator : AbstractValidator<UpdateDoctorCommand>
{
    public UpdateDoctorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Doctor ID is required.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.MedicalSpecialty)
            .NotEmpty().WithMessage("Medical specialty is required.")
            .MaximumLength(50).WithMessage("Medical specialty cannot exceed 50 characters.");

        RuleFor(x => x.ConsultationFee)
            .GreaterThanOrEqualTo(0).WithMessage("Consultation fee cannot be negative.");

        RuleFor(x => x.OfficeAddress)
            .NotEmpty().WithMessage("Office address is required.")
            .MaximumLength(200).WithMessage("Office address cannot exceed 200 characters.");
    }
}
