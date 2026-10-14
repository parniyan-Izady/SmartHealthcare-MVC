using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Staff.Commands.ToggleStaffStatus;

public class ToggleStaffStatusCommandHandler : ICommandHandler<ToggleStaffStatusCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ToggleStaffStatusCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(ToggleStaffStatusCommand request, CancellationToken cancellationToken)
    {
        var staffMember = await _identityService.GetStaffMemberByIdAsync(request.Id, cancellationToken);
        if (staffMember == null)
        {
            throw new NotFoundException("StaffMember", request.Id);
        }

        if (staffMember.Role == UserRole.SuperAdmin)
        {
            throw new ForbiddenAccessException("The SuperAdmin account cannot be deactivated.");
        }

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        if (staffMember.Role == UserRole.Admin && !isSuperAdmin)
        {
            throw new ForbiddenAccessException("Only SuperAdmin can manage Admin accounts.");
        }

        var (succeeded, newStatus, errors) = await _identityService.ToggleUserStatusAsync(request.Id, cancellationToken);
        if (!succeeded)
        {
            var errorMessage = string.Join("; ", errors);
            throw new DomainException(errorMessage);
        }

        return newStatus;
    }
}
