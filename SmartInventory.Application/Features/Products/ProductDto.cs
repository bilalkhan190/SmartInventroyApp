namespace SmartInventory.Application.Features.Products;

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public int ReorderLevel { get; init; }
    public DateTime CreatedAt { get; init; }
}
