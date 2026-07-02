using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, HandlerResult<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.Id && p.DeletedAt == null)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                Quantity = p.ProductInventory != null
                    ? p.ProductInventory.CurrentStockQuantity
                    : p.Quantity,
                ReorderLevel = p.ReorderLevel,
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return HandlerResult<ProductDto>.Failure("Product not found.");
        }

        return HandlerResult<ProductDto>.Success(product);
    }
}
