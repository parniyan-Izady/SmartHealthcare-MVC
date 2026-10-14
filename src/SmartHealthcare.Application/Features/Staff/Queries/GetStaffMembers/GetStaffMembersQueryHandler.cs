using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Staff.Queries.GetStaffMembers;

public class GetStaffMembersQueryHandler : IQueryHandler<GetStaffMembersQuery, List<StaffMemberDto>>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public GetStaffMembersQueryHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<List<StaffMemberDto>> Handle(GetStaffMembersQuery request, CancellationToken cancellationToken)
    {
        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        // Security rule:
        // SuperAdmin can view all Admins and Receptionists.
        // Regular Admin can only view Receptionists.
        var allowedRoles = isSuperAdmin
            ? new[] { UserRole.Admin, UserRole.Receptionist }
            : new[] { UserRole.Receptionist };

        return await _identityService.GetStaffMembersAsync(allowedRoles, cancellationToken);
    }
}
