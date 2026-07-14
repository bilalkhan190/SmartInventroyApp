using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.RequestPurchaseOrderApproval;

public sealed record RequestPurchaseOrderApprovalCommand(Guid Id)
    : IRequest<HandlerResult<PurchaseOrderDto>>;
