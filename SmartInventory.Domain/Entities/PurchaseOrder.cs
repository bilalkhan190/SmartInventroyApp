using SmartInventory.Domain.Common;
using SmartInventory.Domain.Enums;
using SmartInventory.Domain.Events;

namespace SmartInventory.Domain.Entities;


public class PurchaseOrder : BaseEntity
{
    public string OrderNo { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = default!;
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = [];

    public void SubmitForApproval()
    {
        if (Status is not (PurchaseOrderStatus.Rejected or PurchaseOrderStatus.Revised))
        {
            throw new DomainException(
                "Only revised or rejected purchase orders can be submitted for approval.");
        }

        Status = PurchaseOrderStatus.Pending;
        MarkUpdated();
        RaiseDomainEvent(new PurchaseOrderSubmittedForApprovalEvent(Id, OrderNo));
    }


    public void Approve()
    {
        if (Status != PurchaseOrderStatus.Pending)
        {
            throw new DomainException("Only pending purchase orders can be approved.");
        }

        Status = PurchaseOrderStatus.Approved;
        MarkUpdated();
        RaiseDomainEvent(new PurchaseOrderApprovedEvent(Id, OrderNo, SupplierId));
    }


    public void Reject(string? reason = null)
    {
        if (Status != PurchaseOrderStatus.Pending)
        {
            throw new DomainException("Only pending purchase orders can be rejected.");
        }

        Status = PurchaseOrderStatus.Rejected;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            var trimmed = reason.Trim();
            Notes = string.IsNullOrWhiteSpace(Notes)
                ? $"Rejected: {trimmed}"
                : $"{Notes}\nRejected: {trimmed}";
        }

        MarkUpdated();
        RaiseDomainEvent(new PurchaseOrderRejectedEvent(Id, OrderNo, reason?.Trim()));
    }
}
