namespace SmartInventory.Application.Features.Categories;

public sealed class CategoryDto
{
    public Guid Id { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
