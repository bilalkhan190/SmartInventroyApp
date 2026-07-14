using System.ComponentModel.DataAnnotations.Schema;
using SmartInventory.Domain.Common;

namespace SmartInventory.Domain.Entities;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public Guid? DeletedBy { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    /// <summary>
    /// Events raised by domain methods. EF ignores this ([NotMapped]).
    /// Cleared after they are published by the infrastructure dispatcher.
    /// </summary>
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void MarkUpdated() => UpdatedAt = DateTime.UtcNow;

    public void MarkDeleted()
    {
        DeletedAt = DateTime.UtcNow;
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
