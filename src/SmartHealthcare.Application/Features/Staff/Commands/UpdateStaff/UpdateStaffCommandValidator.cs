using FluentValidation;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Staff.Commands.UpdateStaff;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Staff member ID is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{10,15}$").WithMessage("A valid phone number (10 to 15 digits) is required.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("A valid role must be selected.")
            .NotEqual(UserRole.SuperAdmin)
            .WithMessage("SuperAdmin accounts cannot be assigned via the management panel.");
    }
}
