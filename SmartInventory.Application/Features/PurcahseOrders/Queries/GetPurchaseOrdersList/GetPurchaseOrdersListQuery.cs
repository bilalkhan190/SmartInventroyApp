using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrdersList;

namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrdersList;

public sealed record GetPurchaseOrdersListQuery : IRequest<HandlerResult<IReadOnlyList<PurchaseOrderListDto>>>;
