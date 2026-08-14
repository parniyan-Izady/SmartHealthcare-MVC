using SmartHealthcare.Domain.Common;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid IdentityUserId { get; private set; }

    private User() { } // EF Core constructor

    public User(string firstName, string lastName, string email, UserRole role, Guid identityUserId)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Role = role;
        IdentityUserId = identityUserId;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
