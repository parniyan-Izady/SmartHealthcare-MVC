using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface ISignInService
{
    Task<(bool Succeeded, bool IsLockedOut, bool RequiresTwoFactor, string? ErrorMessage)> PasswordSignInAsync(
        string email,
        string password,
        bool isPersistent,
        bool lockoutOnFailure = false);

    Task SignInUserAsync(Guid userId, bool isPersistent = false);

    Task SignOutAsync();
}
