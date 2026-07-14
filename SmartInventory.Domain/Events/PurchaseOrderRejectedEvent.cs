using SmartInventory.Domain.Common;

namespace SmartInventory.Domain.Events;

public sealed record PurchaseOrderRejectedEvent(
    Guid PurchaseOrderId,
    string OrderNo,
    string? Reason) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
