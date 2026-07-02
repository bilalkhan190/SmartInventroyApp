namespace SmartInventory.Application.Features.GoodReceiveNotes.Queries.GetGoodReceiveNotesList;

public sealed class GoodReceiveNoteListDto
{
    public Guid Id { get; init; }
    public string GrnNo { get; init; } = string.Empty;
    public string SupplierName { get; init; } = string.Empty;
    public string PurchaseOrderNo { get; init; } = string.Empty;
    public DateTime ReceiveDate { get; init; }
    public int ItemCount { get; init; }
    public int TotalAcceptedQuantity { get; init; }
}
