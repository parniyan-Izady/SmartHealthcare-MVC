using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default);

    Task<(bool Succeeded, UserDto? User, IEnumerable<string> Errors)> ValidateUserCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
