using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Sku,
    string? Description,
    Guid CategoryId,
    int Quantity,
    int ReorderLevel) : IRequest<HandlerResult<ProductDto>>;
