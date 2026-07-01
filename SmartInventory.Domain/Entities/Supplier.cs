namespace SmartInventory.Domain.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = [];
}
