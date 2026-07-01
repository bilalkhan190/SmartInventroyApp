namespace SmartInventory.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public Guid? DeletedBy { get; protected set; }  
    public DateTime? DeletedAt { get; protected set; }  

    public void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
}
