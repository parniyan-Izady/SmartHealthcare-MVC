using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Infrastructure.Identity;

namespace SmartHealthcare.Infrastructure.Persistence.DbContext;

public class ApplicationDbSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<ApplicationDbSeeder> _logger;

    public ApplicationDbSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<ApplicationDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedSuperAdminAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (UserRole role in Enum.GetValues<UserRole>())
        {
            var roleName = role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var createResult = await _roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });

                if (createResult.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} seeded successfully.", roleName);
                }
                else
                {
                    _logger.LogError("Failed to seed role {RoleName}: {Errors}",
                        roleName,
                        string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private async Task SeedSuperAdminAsync()
    {
        const string superAdminEmail = "superadmin@smarthealthcare.com";

        // Guarantee there is only one SuperAdmin in the entire system, strictly created via DB seeding
        var existingSuperAdmin = await _userManager.Users
            .AnyAsync(u => u.Role == UserRole.SuperAdmin || u.Email == superAdminEmail);

        if (existingSuperAdmin)
        {
            return;
        }

        var superAdmin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = superAdminEmail,
            Email = superAdminEmail,
            EmailConfirmed = true,
            Role = UserRole.SuperAdmin,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        // Default initial credentials for SuperAdmin
        var result = await _userManager.CreateAsync(superAdmin, "SuperAdmin123!");
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(superAdmin, UserRole.SuperAdmin.ToString());
            _logger.LogInformation("SuperAdmin user seeded successfully with email: {Email}", superAdminEmail);
        }
        else
        {
            _logger.LogError("Failed to seed SuperAdmin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
