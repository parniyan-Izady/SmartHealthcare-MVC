using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Features.Staff.Queries.GetStaffMemberById;

public class GetStaffMemberByIdQueryHandler : IQueryHandler<GetStaffMemberByIdQuery, StaffMemberDto>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public GetStaffMemberByIdQueryHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<StaffMemberDto> Handle(GetStaffMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var staffMember = await _identityService.GetStaffMemberByIdAsync(request.Id, cancellationToken);
        if (staffMember == null)
        {
            throw new NotFoundException("StaffMember", request.Id);
        }

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        // Security rule:
        // SuperAdmin account cannot be managed via staff UI.
        // Regular Admin cannot view or manage other Admins or SuperAdmin.
        if (staffMember.Role == UserRole.SuperAdmin || (staffMember.Role == UserRole.Admin && !isSuperAdmin))
        {
            throw new ForbiddenAccessException("You do not have permission to view or edit this staff member.");
        }

        return staffMember;
    }
}
