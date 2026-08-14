using FluentValidation;

namespace SmartHealthcare.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandValidator : AbstractValidator<CreateDoctorCommand>
{
    public CreateDoctorCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.MedicalLicenseNumber)
            .NotEmpty().WithMessage("Medical license number is required.")
            .MaximumLength(30).WithMessage("Medical license number cannot exceed 30 characters.");

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
