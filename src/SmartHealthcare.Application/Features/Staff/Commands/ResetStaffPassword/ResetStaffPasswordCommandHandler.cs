using MediatR;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Staff.Commands.ResetStaffPassword;

public class ResetStaffPasswordCommandHandler : ICommandHandler<ResetStaffPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public ResetStaffPasswordCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ResetStaffPasswordCommand request, CancellationToken cancellationToken)
    {
        var staffMember = await _identityService.GetStaffMemberByIdAsync(request.Id, cancellationToken);
        if (staffMember == null)
        {
            throw new NotFoundException("StaffMember", request.Id);
        }

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        // Security check:
        // SuperAdmin password cannot be reset via the staff management panel.
        // Admin cannot reset password for other Admins or SuperAdmin.
        if (staffMember.Role == UserRole.SuperAdmin || (staffMember.Role == UserRole.Admin && !isSuperAdmin))
        {
            throw new ForbiddenAccessException("You do not have permission to reset the password for this account.");
        }

        var (succeeded, errors) = await _identityService.ResetPasswordAsync(request.Id, request.NewPassword, cancellationToken);
        if (!succeeded)
        {
            var errorMessage = string.Join("; ", errors);
            throw new DomainException(errorMessage);
        }

        return Unit.Value;
    }
}
