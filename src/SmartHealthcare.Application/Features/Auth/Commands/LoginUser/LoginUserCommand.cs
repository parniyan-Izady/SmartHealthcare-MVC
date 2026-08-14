using SmartHealthcare.Application.Common.CQRS;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Features.Auth.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password
) : ICommand<AuthResponse>;
