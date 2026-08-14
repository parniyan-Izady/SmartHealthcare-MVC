using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IIdentityService identityService,
        IApplicationDbContext dbContext,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, identityUserId, errors) = await _identityService.ValidateUserCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!succeeded)
        {
            throw new DomainException("Invalid email or password.");
        }

        var domainUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId, cancellationToken)
            ?? await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (domainUser == null)
        {
            throw new DomainException("User account profile not found.");
        }

        var token = _jwtTokenGenerator.GenerateToken(domainUser);
        return new AuthResponse(domainUser.Id, $"{domainUser.FirstName} {domainUser.LastName}", domainUser.Email, domainUser.Role.ToString(), token);
    }
}
