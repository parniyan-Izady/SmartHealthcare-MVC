using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Staff.Queries.GetStaffMemberById;

public record GetStaffMemberByIdQuery(Guid Id) : IQuery<StaffMemberDto>;
