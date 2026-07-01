namespace SmartInventory.Application.Features.Suppliers;

public sealed class SupplierDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ContactName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public DateTime CreatedAt { get; init; }
}
