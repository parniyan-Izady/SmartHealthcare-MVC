using AutoMapper;
using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(
        IIdentityService identityService,
        IJwtTokenGenerator jwtTokenGenerator,
        IMapper mapper)
    {
        _identityService = identityService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _identityService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new DomainException($"Email '{request.Email}' is already registered.");
        }

        var (succeeded, userId, errors) = await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.Role,
            cancellationToken);

        if (!succeeded)
        {
            var errorList = string.Join(", ", errors);
            throw new DomainException($"User registration failed: {errorList}");
        }

        var user = await _identityService.GetUserByIdAsync(userId, cancellationToken)
            ?? throw new DomainException("Created user could not be retrieved.");

        var token = _jwtTokenGenerator.GenerateToken(user);
        return _mapper.Map<AuthResponse>(user, opt => opt.Items["Token"] = token);
    }
}
