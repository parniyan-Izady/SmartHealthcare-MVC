using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Staff.Commands.CreateStaff;

public class CreateStaffCommandHandler : ICommandHandler<CreateStaffCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly ICurrentUserService _currentUserService;

    public CreateStaffCommandHandler(
        IIdentityService identityService,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        // 1. Authorization check: Non-SuperAdmin cannot create an Admin
        if (request.Role == UserRole.Admin && !_currentUserService.IsInRole(UserRole.SuperAdmin))
        {
            throw new ForbiddenAccessException("Only SuperAdmin has permission to create new Admin accounts.");
        }

        // 2. Uniqueness check for email
        var existingUser = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new DomainException("A user with this email address already exists.");
        }

        // 3. Create staff user
        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.Role,
            request.PhoneNumber,
            cancellationToken);

        if (!succeeded)
        {
            var errorMessage = string.Join("; ", errors);
            throw new DomainException($"Failed to create staff account: {errorMessage}");
        }

        return userId;
    }
}
