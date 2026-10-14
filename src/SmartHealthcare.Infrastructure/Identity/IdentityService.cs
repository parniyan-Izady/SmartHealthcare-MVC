using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

    public Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email,
        string password,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        return CreateUserAsync(email, password, role, null, cancellationToken);
    }

    public async Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(
        string email,
        string password,
        UserRole role,
        string? phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = role,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, Guid.Empty, result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, role.ToString());

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

    public async Task<List<StaffMemberDto>> GetStaffMembersAsync(
        IEnumerable<UserRole> roles,
        CancellationToken cancellationToken = default)
    {
        var roleList = roles.ToList();

        return await _userManager.Users
            .Where(u => roleList.Contains(u.Role))
            .OrderBy(u => u.Role)
            .ThenByDescending(u => u.CreatedAtUtc)
            .Select(u => new StaffMemberDto(
                u.Id,
                u.Email ?? string.Empty,
                u.PhoneNumber,
                u.Role,
                u.IsActive,
                u.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<StaffMemberDto?> GetStaffMemberByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return null;

        return new StaffMemberDto(
            user.Id,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.Role,
            user.IsActive,
            user.CreatedAtUtc);
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateStaffMemberAsync(
        UpdateStaffMemberDto dto,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user == null)
        {
            return (false, new[] { "Staff member not found." });
        }

        // Email uniqueness check if changed
        if (!string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailInUse = await _userManager.FindByEmailAsync(dto.Email);
            if (emailInUse != null && emailInUse.Id != user.Id)
            {
                return (false, new[] { "This email is already in use by another account." });
            }

            user.Email = dto.Email;
            user.UserName = dto.Email;
        }

        user.PhoneNumber = dto.PhoneNumber;
        user.UpdatedAtUtc = DateTime.UtcNow;

        if (dto.Role != user.Role && dto.Role != UserRole.SuperAdmin)
        {
            await _userManager.RemoveFromRoleAsync(user, user.Role.ToString());
            await _userManager.AddToRoleAsync(user, dto.Role.ToString());
            user.Role = dto.Role;
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description));
        }

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(
        Guid userId,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, new[] { "Staff member not found." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description));
        }

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, bool NewStatus, IEnumerable<string> Errors)> ToggleUserStatusAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return (false, false, new[] { "Staff member not found." });
        }

        var newStatus = !user.IsActive;
        user.SetActiveStatus(newStatus);
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return (false, user.IsActive, result.Errors.Select(e => e.Description));
        }

        return (true, newStatus, Array.Empty<string>());
    }

    private static UserDto ToUserDto(ApplicationUser user) => new(
        user.Id,
        user.Email,
        user.Role,
        user.IsActive
    );
}
