using FluentValidation;

namespace SmartHealthcare.Application.Features.Doctors.Queries.GetPagedDoctors;

public class GetPagedDoctorsQueryValidator : AbstractValidator<GetPagedDoctorsQuery>
{
    private static readonly string[] AllowedSortColumns = ["LastName", "FirstName", "MedicalSpecialty", "ConsultationFee"];
    private static readonly string[] AllowedSortOrders = ["asc", "desc"];

    public GetPagedDoctorsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SortBy)
            .Must(sort => string.IsNullOrEmpty(sort) || AllowedSortColumns.Contains(sort, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Sort column must be one of the following: {string.Join(", ", AllowedSortColumns)}.");

        RuleFor(x => x.SortOrder)
            .Must(order => string.IsNullOrEmpty(order) || AllowedSortOrders.Contains(order, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort order must be either 'asc' or 'desc'.");
    }
}
