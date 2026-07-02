using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderById;

public sealed record GetPurchaseOrderByIdQuery(Guid Id) : IRequest<HandlerResult<PurchaseOrderDto>>;
