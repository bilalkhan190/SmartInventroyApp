using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Products.Queries.GetProductsList;

public sealed record GetProductsListQuery : IRequest<HandlerResult<IReadOnlyList<ProductDto>>>;
