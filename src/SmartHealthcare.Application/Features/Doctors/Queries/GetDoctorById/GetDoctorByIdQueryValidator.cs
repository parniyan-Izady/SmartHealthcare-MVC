using FluentValidation;

namespace SmartHealthcare.Application.Features.Doctors.Queries.GetDoctorById;

public class GetDoctorByIdQueryValidator : AbstractValidator<GetDoctorByIdQuery>
{
    public GetDoctorByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Doctor ID is required.")
            .NotEqual(Guid.Empty).WithMessage("Doctor ID cannot be empty.");
    }
}
