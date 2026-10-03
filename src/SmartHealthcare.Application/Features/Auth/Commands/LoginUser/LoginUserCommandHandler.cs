using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Auth.Commands.LoginUser;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginUserCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var (succeeded, user, errors) = await _identityService.ValidateUserCredentialsAsync(request.Email, request.Password, cancellationToken);
        if (!succeeded || user == null)
        {
            throw new DomainException("Invalid email or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        return _mapper.Map<AuthResponse>(user, opt => opt.Items["Token"] = token);
    }
}
