using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        IIdentityService identityService,
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, identityUserId, errors) = await _identityService.ValidateUserCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!succeeded)
        {
            throw new DomainException("Invalid email or password.");
        }

        var domainUser = await _userRepository.GetByIdentityUserIdAsync(identityUserId, cancellationToken);

        if (domainUser == null)
        {
            throw new DomainException("User account profile not found.");
        }

        var token = _jwtTokenGenerator.GenerateToken(domainUser);
        return new AuthResponse(domainUser.Id, $"{domainUser.FirstName} {domainUser.LastName}", domainUser.Email, domainUser.Role.ToString(), token);
    }
}
