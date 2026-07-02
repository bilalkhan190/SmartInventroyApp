namespace SmartInventory.Application.Features.Inventory.Queries.GetInventoriesList;

public sealed class InventoryListDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public int CurrentStockQuantity { get; init; }
    public int ReorderLevel { get; init; }
    public bool IsLowStock { get; init; }
}
