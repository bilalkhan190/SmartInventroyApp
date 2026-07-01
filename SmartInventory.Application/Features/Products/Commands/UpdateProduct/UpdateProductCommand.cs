using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Sku,
    string? Description,
    Guid CategoryId,
    int Quantity,
    int ReorderLevel) : IRequest<HandlerResult<ProductDto>>;
