using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommandHandler(
        IIdentityService identityService,
        IApplicationDbContext dbContext,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new DomainException($"Email '{request.Email}' is already registered.");
        }

        // Phase 1: Create Identity Security User
        var (succeeded, identityUserId, errors) = await _identityService.CreateIdentityUserAsync(request.Email, request.Password, cancellationToken);
        if (!succeeded)
        {
            var errorList = string.Join(", ", errors);
            throw new DomainException($"User registration failed: {errorList}");
        }

        // Phase 2: Create Domain User Entity
        var domainUser = new User(request.FirstName, request.LastName, request.Email, request.Role, identityUserId);

        try
        {
            _dbContext.Users.Add(domainUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Compensation: Rollback Identity User on Domain save failure
            await _identityService.DeleteIdentityUserAsync(identityUserId, cancellationToken);
            throw new DomainException($"User registration failed during domain profile creation: {ex.Message}");
        }

        var token = _jwtTokenGenerator.GenerateToken(domainUser);
        return new AuthResponse(domainUser.Id, $"{domainUser.FirstName} {domainUser.LastName}", domainUser.Email, domainUser.Role.ToString(), token);
    }
}
