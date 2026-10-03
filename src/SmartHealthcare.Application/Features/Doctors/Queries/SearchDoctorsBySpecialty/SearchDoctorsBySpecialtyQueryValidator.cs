using FluentValidation;

namespace SmartHealthcare.Application.Features.Doctors.Queries.SearchDoctorsBySpecialty;

public class SearchDoctorsBySpecialtyQueryValidator : AbstractValidator<SearchDoctorsBySpecialtyQuery>
{
    public SearchDoctorsBySpecialtyQueryValidator()
    {
        RuleFor(x => x.Specialty)
            .NotEmpty().WithMessage("Medical specialty is required.")
            .MinimumLength(2).WithMessage("Medical specialty must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Medical specialty cannot exceed 100 characters.");
    }
}
