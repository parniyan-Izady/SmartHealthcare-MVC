using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Doctors.Queries.GetPagedDoctors;

public record GetPagedDoctorsQuery(
    string? Specialty = null,
    string? SearchTerm = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = "LastName",
    string? SortOrder = "asc"
) : IQuery<PagedResult<DoctorResponse>>;
