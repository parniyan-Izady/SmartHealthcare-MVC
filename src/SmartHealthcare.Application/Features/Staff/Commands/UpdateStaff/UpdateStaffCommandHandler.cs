using MediatR;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Staff.Commands.UpdateStaff;

public class UpdateStaffCommandHandler : ICommandHandler<UpdateStaffCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public UpdateStaffCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
    {
        var existingStaff = await _identityService.GetStaffMemberByIdAsync(request.Id, cancellationToken);
        if (existingStaff == null)
        {
            throw new NotFoundException("StaffMember", request.Id);
        }

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        // Security check 1: Target account authorization
        // SuperAdmin cannot be edited via Staff management.
        // Admin cannot edit another Admin or SuperAdmin.
        if (existingStaff.Role == UserRole.SuperAdmin || (existingStaff.Role == UserRole.Admin && !isSuperAdmin))
        {
            throw new ForbiddenAccessException("You do not have permission to edit this account.");
        }

        // Security check 2: Role change authorization
        if (request.Role != existingStaff.Role && !isSuperAdmin)
        {
            throw new ForbiddenAccessException("Only SuperAdmin has permission to change staff roles.");
        }

        var targetRole = isSuperAdmin ? request.Role : existingStaff.Role;

        var updateDto = new UpdateStaffMemberDto(
            request.Id,
            request.Email,
            request.PhoneNumber,
            targetRole
        );

        var (succeeded, errors) = await _identityService.UpdateStaffMemberAsync(updateDto, cancellationToken);

        if (!succeeded)
        {
            var errorMessage = string.Join("; ", errors);
            throw new DomainException(errorMessage);
        }

        return Unit.Value;
    }
}
