using SmartInventory.Domain.Enums;

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
}
