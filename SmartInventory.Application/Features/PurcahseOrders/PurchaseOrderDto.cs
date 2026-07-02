namespace SmartInventory.Application.Features.PurcahseOrders;

public sealed class PurchaseOrderDto
{
    public Guid Id { get; init; }
    public string OrderNo { get; init; } = string.Empty;
    public Guid SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<PurchaseOrderItemDto> Items { get; init; } = [];
}

public sealed class PurchaseOrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitAmount { get; init; }
}
