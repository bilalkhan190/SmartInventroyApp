using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.RejectPurchaseOrder;

public sealed record RejectPurchaseOrderCommand(Guid Id, string? Reason = null)
    : IRequest<HandlerResult<PurchaseOrderDto>>;
