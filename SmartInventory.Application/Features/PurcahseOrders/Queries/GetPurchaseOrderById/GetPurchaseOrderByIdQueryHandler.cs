using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Application.Features.PurcahseOrders;

namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderById;

public sealed class GetPurchaseOrderByIdQueryHandler
    : IRequestHandler<GetPurchaseOrderByIdQuery, HandlerResult<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrderByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<PurchaseOrderDto>> Handle(
        GetPurchaseOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .AsNoTracking()
            .Where(order => order.Id == request.Id && order.DeletedAt == null)
            .Select(order => new PurchaseOrderDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                SupplierId = order.SupplierId,
                SupplierName = order.Supplier.Name,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                Notes = order.Notes,
                Items = order.Items
                    .Where(item => item.DeletedAt == null)
                    .Select(item => new PurchaseOrderItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Sku = item.Product.Sku,
                        Quantity = item.Quantity,
                        UnitAmount = item.UnitAmount
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (purchaseOrder is null)
        {
            return HandlerResult<PurchaseOrderDto>.Failure("Purchase order not found.");
        }

        return HandlerResult<PurchaseOrderDto>.Success(purchaseOrder);
    }
}
