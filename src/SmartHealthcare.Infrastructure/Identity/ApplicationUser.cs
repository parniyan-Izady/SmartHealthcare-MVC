using Microsoft.AspNetCore.Identity;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }
}
