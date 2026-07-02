namespace SmartInventory.Domain.Entities;

public class Inventory : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public int CurrentStockQuantity { get; set; }
}
