namespace SmartInventory.Domain.Entities;

public class GoodReceiveNoteItem : BaseEntity
{
    public Guid GoodReceiveNoteId { get; set; }
    public GoodReceiveNote GoodReceiveNote { get; set; } = default!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public int OrderedQuantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public int AcceptedQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? BatchNo { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public string? Remarks { get; set; }
}
