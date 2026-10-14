using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Staff.Queries.GetStaffMembers;

public record GetStaffMembersQuery : IQuery<List<StaffMemberDto>>;
