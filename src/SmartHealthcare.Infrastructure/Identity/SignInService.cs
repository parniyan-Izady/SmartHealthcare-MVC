using Microsoft.AspNetCore.Identity;
using SmartHealthcare.Application.Common.Interfaces;
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
