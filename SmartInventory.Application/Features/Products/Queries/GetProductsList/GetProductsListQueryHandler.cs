using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Products.Queries.GetProductsList;

public sealed class GetProductsListQueryHandler
    : IRequestHandler<GetProductsListQuery, HandlerResult<IReadOnlyList<ProductDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<ProductDto>>> Handle(
        GetProductsListQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _context.Products
            .AsNoTracking()
            .Where(product => product.DeletedAt == null)
            .OrderBy(product => product.Name)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                Quantity = product.ProductInventory != null
                    ? product.ProductInventory.CurrentStockQuantity
                    : product.Quantity,
                ReorderLevel = product.ReorderLevel,
                CreatedAt = product.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<ProductDto>>.Success(products);
    }
}
