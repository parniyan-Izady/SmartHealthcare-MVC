using MediatR;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Staff.Commands.DeleteStaff;

public class DeleteStaffCommandHandler : ICommandHandler<DeleteStaffCommand>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public DeleteStaffCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
    {
        var staffMember = await _identityService.GetStaffMemberByIdAsync(request.Id, cancellationToken);
        if (staffMember == null)
        {
            throw new NotFoundException("StaffMember", request.Id);
        }

        if (staffMember.Role == UserRole.SuperAdmin)
        {
            throw new ForbiddenAccessException("The SuperAdmin account cannot be deleted.");
        }

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        if (staffMember.Role == UserRole.Admin && !isSuperAdmin)
        {
            throw new ForbiddenAccessException("Only SuperAdmin can delete Admin accounts.");
        }

        var succeeded = await _identityService.DeleteUserAsync(request.Id, cancellationToken);
        if (!succeeded)
        {
            throw new DomainException("Failed to delete staff member.");
        }

        return Unit.Value;
    }
}
