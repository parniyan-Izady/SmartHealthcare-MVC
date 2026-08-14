using Microsoft.AspNetCore.Identity;
using SmartHealthcare.Application.Common.Interfaces;

namespace SmartHealthcare.Infrastructure.Identity;


// Note:
// ASP.NET Core Identity's RoleManager and SignInManager are not used here.
// Roles are managed as a Domain enum (UserRole) because they are part of the
// application's business rules, while Identity is responsible only for security concerns.
// Sign-in is handled through UserManager because this service only needs to validate credentials.
// If the project later requires Identity-managed roles or full sign-in/lockout workflows,
// RoleManager and SignInManager can be introduced.
public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(bool Succeeded, Guid IdentityUserId, IEnumerable<string> Errors)> CreateIdentityUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return (false, Guid.Empty, result.Errors.Select(e => e.Description));
        }

        return (true, user.Id, Enumerable.Empty<string>());
    }

    public async Task<(bool Succeeded, Guid IdentityUserId, IEnumerable<string> Errors)> ValidateUserCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false, Guid.Empty, new[] { "User not found." });
        }

        var isValid = await _userManager.CheckPasswordAsync(user, password);
        if (!isValid)
        {
            return (false, Guid.Empty, new[] { "Invalid password." });
        }

        return (true, user.Id, Enumerable.Empty<string>());
    }

    public async Task<bool> DeleteIdentityUserAsync(Guid identityUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityUserId.ToString());
        if (user == null) return true;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}

/*
===============================================================================
 FUTURE VERSION
 If Identity Roles and SignInManager are introduced later
===============================================================================

// using Microsoft.AspNetCore.Identity;
// using SmartHealthcare.Application.Common.Interfaces;

// namespace SmartHealthcare.Infrastructure.Identity;

// public class IdentityService : IIdentityService
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly SignInManager<ApplicationUser> _signInManager;
//     private readonly RoleManager<IdentityRole<Guid>> _roleManager;

//     public IdentityService(
//         UserManager<ApplicationUser> userManager,
//         SignInManager<ApplicationUser> signInManager,
//         RoleManager<IdentityRole<Guid>> roleManager)
//     {
//         _userManager = userManager;
//         _signInManager = signInManager;
//         _roleManager = roleManager;
//     }

//     // -----------------------------------------------------------------------
//     // Register
//     // -----------------------------------------------------------------------

//     public async Task<...> CreateIdentityUserAsync(...)
//     {
//         var user = new ApplicationUser
//         {
//             Id = Guid.NewGuid(),
//             UserName = email,
//             Email = email
//         };

//         var result = await _userManager.CreateAsync(user, password);

//         if (!result.Succeeded)
//         {
//             return (...);
//         }

//         // Assign Identity Role
//         await _userManager.AddToRoleAsync(user, "Doctor");

//         return (...);
//     }

//     // -----------------------------------------------------------------------
//     // Login
//     // -----------------------------------------------------------------------

//     public async Task<...> ValidateUserCredentialsAsync(...)
//     {
//         var user = await _userManager.FindByEmailAsync(email);

//         if (user == null)
//         {
//             return (...);
//         }

//         // SignInManager handles the sign-in workflow.
//         var result = await _signInManager.CheckPasswordSignInAsync(
//             user,
//             password,
//             lockoutOnFailure: true
//         );

//         if (!result.Succeeded)
//         {
//             return (...);
//         }

//         return (...);
//     }

//     // -----------------------------------------------------------------------
//     // Role Management
//     // -----------------------------------------------------------------------

//     public async Task<bool> CreateRoleAsync(string roleName)
//     {
//         if (await _roleManager.RoleExistsAsync(roleName))
//             return true;

//         var result = await _roleManager.CreateAsync(
//             new IdentityRole<Guid>(roleName)
//         );

//         return result.Succeeded;
//     }

//     public async Task<bool> AssignRoleAsync(
//         Guid identityUserId,
//         string roleName)
//     {
//         var user = await _userManager.FindByIdAsync(
//             identityUserId.ToString()
//         );

//         if (user == null)
//             return false;

//         var result = await _userManager.AddToRoleAsync(
//             user,
//             roleName
//         );

//         return result.Succeeded;
//     }
// }
*/
