using Microsoft.AspNetCore.Identity;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            Role = role,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, Guid.Empty, result.Errors.Select(e => e.Description));
        }

        return (true, user.Id, Enumerable.Empty<string>());
    }

    public async Task<(bool Succeeded, UserDto? User, IEnumerable<string> Errors)> ValidateUserCredentialsAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, null, new[] { "User not found." });
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isValid)
        {
            return (false, null, new[] { "Invalid password." });
        }

        return (true, ToUserDto(user), Enumerable.Empty<string>());
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user == null ? null : ToUserDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user == null ? null : ToUserDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return true;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    private static UserDto ToUserDto(ApplicationUser user) => new(
        user.Id,
        user.Email,
        user.Role,
        user.IsActive
    );
}
