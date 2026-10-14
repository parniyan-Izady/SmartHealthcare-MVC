using FluentValidation;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Staff.Commands.CreateStaff;

public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("A valid phone number (10 to 15 digits) is required.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("A valid role must be selected.")
            .NotEqual(UserRole.SuperAdmin)
            .WithMessage("SuperAdmin accounts can only be provisioned directly via the database.");
    }
}
