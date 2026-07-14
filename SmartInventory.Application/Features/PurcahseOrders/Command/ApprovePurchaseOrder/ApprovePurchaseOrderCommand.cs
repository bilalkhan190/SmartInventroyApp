using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.ApprovePurchaseOrder;

public sealed record ApprovePurchaseOrderCommand(Guid Id)
    : IRequest<HandlerResult<PurchaseOrderDto>>;
