using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderForApproval;

public sealed record GetPurchaseOrderForApprovalQuery()
    : IRequest<HandlerResult<IReadOnlyList<PurchaseOrderDto>>>;
