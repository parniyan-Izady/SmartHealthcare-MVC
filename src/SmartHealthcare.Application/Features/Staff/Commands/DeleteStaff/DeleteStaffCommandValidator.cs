using FluentValidation;

namespace SmartHealthcare.Application.Features.Staff.Commands.DeleteStaff;

public class DeleteStaffCommandValidator : AbstractValidator<DeleteStaffCommand>
{
    public DeleteStaffCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Staff member ID is required.");
    }
}
