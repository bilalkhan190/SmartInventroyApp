using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.CreatePurcahseOrder;

public sealed record CreatePurchaseOrderCommand(
    Guid SupplierId,
    IReadOnlyList<PurchaseOrderProductInfo> Products,
    string? Notes = null) : IRequest<HandlerResult<PurchaseOrderDto>>;

public sealed record PurchaseOrderProductInfo(
    Guid ProductId,
    int Quantity,
    decimal UnitAmount);
