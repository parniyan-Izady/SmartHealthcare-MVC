using FluentValidation;

namespace SmartHealthcare.Application.Features.Staff.Commands.ResetStaffPassword;

public class ResetStaffPasswordCommandValidator : AbstractValidator<ResetStaffPasswordCommand>
{
    public ResetStaffPasswordCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Staff member ID is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
