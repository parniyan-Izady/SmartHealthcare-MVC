using FluentValidation;

namespace SmartHealthcare.Application.Features.Appointments.Queries.GetPagedAppointments;

public class GetPagedAppointmentsQueryValidator : AbstractValidator<GetPagedAppointmentsQuery>
{
    private static readonly string[] AllowedSortOrders = ["asc", "desc"];

    public GetPagedAppointmentsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.SortOrder)
            .Must(order => string.IsNullOrEmpty(order) || AllowedSortOrders.Contains(order, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Sort order must be either 'asc' or 'desc'.");

        RuleFor(x => x.DoctorId)
            .NotEqual(Guid.Empty).When(x => x.DoctorId.HasValue)
            .WithMessage("Doctor ID cannot be empty.");

        RuleFor(x => x.PatientId)
            .NotEqual(Guid.Empty).When(x => x.PatientId.HasValue)
            .WithMessage("Patient ID cannot be empty.");

        RuleFor(x => x.ToDateUtc)
            .GreaterThanOrEqualTo(x => x.FromDateUtc!.Value)
            .When(x => x.FromDateUtc.HasValue && x.ToDateUtc.HasValue)
            .WithMessage("End date must be greater than or equal to start date.");
    }
}
