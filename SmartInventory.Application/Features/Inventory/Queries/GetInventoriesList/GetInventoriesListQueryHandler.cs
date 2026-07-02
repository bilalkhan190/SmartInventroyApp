using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Inventory.Queries.GetInventoriesList;

public sealed class GetInventoriesListQueryHandler
    : IRequestHandler<GetInventoriesListQuery, HandlerResult<IReadOnlyList<InventoryListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoriesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<InventoryListDto>>> Handle(
        GetInventoriesListQuery request,
        CancellationToken cancellationToken)
    {
        var inventories = await _context.Inventories
            .AsNoTracking()
            .Where(inventory => inventory.DeletedAt == null)
            .OrderBy(inventory => inventory.Product.Name)
            .Select(inventory => new InventoryListDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                ProductName = inventory.Product.Name,
                Sku = inventory.Product.Sku,
                CategoryName = inventory.Product.Category.CategoryName,
                CurrentStockQuantity = inventory.CurrentStockQuantity,
                ReorderLevel = inventory.Product.ReorderLevel,
                IsLowStock = inventory.CurrentStockQuantity <= inventory.Product.ReorderLevel
            })
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<InventoryListDto>>.Success(inventories);
    }
}
