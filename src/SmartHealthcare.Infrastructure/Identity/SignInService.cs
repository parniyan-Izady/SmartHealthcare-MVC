using Microsoft.AspNetCore.Identity;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;
using System.Security.Claims;

namespace SmartHealthcare.Infrastructure.Identity;

public class SignInService : ISignInService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public SignInService(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<(bool Succeeded, bool IsLockedOut, bool RequiresTwoFactor, string? ErrorMessage)> PasswordSignInAsync(
        string email,
        string password,
        bool isPersistent,
        bool lockoutOnFailure = false)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, false, false, "Invalid email or password.");
        }

        if (!user.IsActive)
        {
            return (false, false, false, "Your account has been deactivated. Please contact support.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName ?? user.Email!,
            password,
            isPersistent,
            lockoutOnFailure);

        if (result.Succeeded)
        {
            await RefreshCustomClaimsAsync(user, isPersistent);
            return (true, false, false, null);
        }

        if (result.IsLockedOut)
        {
            return (false, true, false, "Account is locked out.");
        }

        if (result.RequiresTwoFactor)
        {
            return (false, false, true, "Two-factor authentication required.");
        }

        return (false, false, false, "Invalid email or password.");
    }

    public async Task SignInUserAsync(Guid userId, bool isPersistent = false)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user != null)
        {
            await _signInManager.SignInAsync(user, isPersistent);
            await RefreshCustomClaimsAsync(user, isPersistent);
        }
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    /// Gets the current user's info after they pass the first step (password authentication).
    public async Task<UserDto?> GetTwoFactorAuthenticationUserAsync()
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return null;
        }

        return new UserDto(
            user.Id,
            user.Email,
            user.Role,
            user.IsActive);
    }

    /// Finds the user by email and generates a one-time OTP code for email or SMS delivery.
    public async Task<string> GenerateTwoFactorTokenAsync(string email, string provider = "Email")
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new InvalidOperationException($"User with email '{email}' not found.");
        }

        return await _userManager.GenerateTwoFactorTokenAsync(user, provider);
    }

    /// Verifies the 2FA state and entered code:
    /// - If valid: signs in and refreshes custom claims.
    /// - If failed: returns account lockout or invalid code error.
    public async Task<(bool Succeeded, bool IsLockedOut, string? ErrorMessage)> TwoFactorSignInAsync(
        string provider,
        string code,
        bool isPersistent,
        bool rememberClient)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            return (false, false, "Unable to load two-factor authentication user.");
        }

        var result = await _signInManager.TwoFactorSignInAsync(provider, code, isPersistent, rememberClient);

        if (result.Succeeded)
        {
            await RefreshCustomClaimsAsync(user, isPersistent);
            return (true, false, null);
        }

        if (result.IsLockedOut)
        {
            return (false, true, "Account is locked out.");
        }

        return (false, false, "Invalid verification code.");
    }

    private async Task RefreshCustomClaimsAsync(ApplicationUser user, bool isPersistent)
    {
        var additionalClaims = new List<Claim>
        {
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.Role == UserRole.Patient)
        {
            var patient = await _patientRepository.GetByUserIdAsync(user.Id);
            if (patient != null)
            {
                additionalClaims.Add(new Claim("PatientId", patient.Id.ToString()));
                additionalClaims.Add(new Claim(ClaimTypes.Name, $"{patient.FirstName} {patient.LastName}"));
            }
        }
        else if (user.Role == UserRole.Doctor)
        {
            var doctor = await _doctorRepository.GetByUserIdAsync(user.Id);
            if (doctor != null)
            {
                additionalClaims.Add(new Claim("DoctorId", doctor.Id.ToString()));
                additionalClaims.Add(new Claim(ClaimTypes.Name, $"{doctor.FirstName} {doctor.LastName}"));
            }
        }

        if (additionalClaims.Count > 0)
        {
            // Re-issue cookie with custom claims (DoctorId/PatientId/Name) to avoid extra DB queries.
            await _signInManager.SignInWithClaimsAsync(user, isPersistent, additionalClaims);
        }
    }
}
