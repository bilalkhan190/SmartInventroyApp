namespace SmartInventory.Application.Features.GoodReceiveNotes;

public sealed class GoodReceiveNoteDto
{
    public Guid Id { get; init; }
    public string GrnNo { get; init; } = string.Empty;
    public Guid SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public Guid PurchaseOrderId { get; init; }
    public string PurchaseOrderNo { get; init; } = string.Empty;
    public DateTime ReceiveDate { get; init; }
    public IReadOnlyList<GoodReceiveNoteItemDto> Items { get; init; } = [];
}

public sealed class GoodReceiveNoteItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public int OrderedQuantity { get; init; }
    public int ReceivedQuantity { get; init; }
    public int AcceptedQuantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string? BatchNo { get; init; }
    public DateTime? ExpiredAt { get; init; }
    public string? Remarks { get; init; }
}
