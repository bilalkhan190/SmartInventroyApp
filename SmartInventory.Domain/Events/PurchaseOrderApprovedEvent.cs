using SmartInventory.Domain.Common;

namespace SmartInventory.Domain.Events;

public sealed record PurchaseOrderApprovedEvent(
    Guid PurchaseOrderId,
    string OrderNo,
    Guid SupplierId) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
