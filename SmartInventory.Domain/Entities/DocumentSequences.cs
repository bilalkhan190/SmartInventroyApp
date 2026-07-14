using SmartInventory.Domain.Enums;

namespace SmartInventory.Domain.Entities;

public class DocumentSequences
{
    public Guid DocumentSequenceId { get; set; } = Guid.NewGuid();
    public DocumentTypes DocumentType { get; set; }
    public int Year { get; set; }
    public int LastNumber { get; set; }
    public string Prefix { get; set; } = string.Empty;
}
