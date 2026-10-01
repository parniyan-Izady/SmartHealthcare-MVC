namespace SmartHealthcare.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; protected set; }
    public DateTime? UpdatedAtUtc { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }

    public bool IsDeleted { get; protected set; } = false;
    public DateTime? DeletedAtUtc { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    public void MarkCreated(Guid? createdBy = null)
    {
        CreatedAtUtc = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void MarkUpdated(Guid? updatedBy = null)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void MarkDeleted(Guid? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }
}
