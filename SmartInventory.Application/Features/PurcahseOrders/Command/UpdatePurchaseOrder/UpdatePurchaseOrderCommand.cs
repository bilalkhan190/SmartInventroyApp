using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;
using SmartInventory.Application.Features.PurcahseOrders.Command.CreatePurcahseOrder;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.UpdatePurchaseOrder;

public sealed record UpdatePurchaseOrderCommand(
    Guid Id,
    Guid SupplierId,
    IReadOnlyList<PurchaseOrderProductInfo> Products,
    string? Notes = null) : IRequest<HandlerResult<PurchaseOrderDto>>;
