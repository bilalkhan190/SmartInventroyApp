using SmartInventory.Domain.Common;

namespace SmartInventory.Domain.Events;

public sealed record PurchaseOrderSubmittedForApprovalEvent(
    Guid PurchaseOrderId,
    string OrderNo) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}
