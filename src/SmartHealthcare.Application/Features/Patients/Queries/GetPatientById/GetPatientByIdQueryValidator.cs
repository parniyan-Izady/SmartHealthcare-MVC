using FluentValidation;

namespace SmartHealthcare.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryValidator : AbstractValidator<GetPatientByIdQuery>
{
    public GetPatientByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Patient ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Patient ID cannot be empty.");
    }
}
