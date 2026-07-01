using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<HandlerResult<ProductDto>>;
