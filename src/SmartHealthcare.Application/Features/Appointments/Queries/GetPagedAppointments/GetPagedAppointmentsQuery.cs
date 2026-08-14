using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Appointments.Queries.GetPagedAppointments;

public record GetPagedAppointmentsQuery(
    Guid? DoctorId = null,
    Guid? PatientId = null,
    string? Status = null,
    DateTime? FromDateUtc = null,
    DateTime? ToDateUtc = null,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = "StartUtc",
    string? SortOrder = "asc"
) : IQuery<PagedResult<AppointmentResponse>>;
