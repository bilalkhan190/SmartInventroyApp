namespace SmartInventory.Domain.Entities;

public class GoodReceiveNote : BaseEntity
{
    public string GrnNo { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Supplier Supplier { get; set; } = default!;
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public DateTime ReceiveDate { get; set; }
    public Guid ReceivedBy { get; set; }

    public ICollection<GoodReceiveNoteItem> GoodReceiveNoteItems { get; set; } = [];
}
