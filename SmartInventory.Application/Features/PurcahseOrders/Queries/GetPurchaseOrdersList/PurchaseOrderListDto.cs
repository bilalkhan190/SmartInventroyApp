namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrdersList;

public sealed class PurchaseOrderListDto
{
    public Guid Id { get; init; }
    public string OrderNo { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public int ItemCount { get; init; }
    public decimal TotalAmount { get; init; }
}
