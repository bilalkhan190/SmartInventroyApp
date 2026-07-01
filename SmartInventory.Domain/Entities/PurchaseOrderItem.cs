namespace SmartInventory.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitAmount { get; set; }
}
