using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Application.Features.PurcahseOrders;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderForApproval;

public sealed class GetPurchaseOrderForApprovalHandler
    : IRequestHandler<GetPurchaseOrderForApprovalQuery, HandlerResult<IReadOnlyList<PurchaseOrderDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrderForApprovalHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<PurchaseOrderDto>>> Handle(
        GetPurchaseOrderForApprovalQuery request,
        CancellationToken cancellationToken)
    {
        var purchaseOrders = await _context.PurchaseOrders
            .AsNoTracking()
            .Where(order => order.Status == PurchaseOrderStatus.Pending && order.DeletedAt == null)
            .OrderByDescending(order => order.OrderDate)
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
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<PurchaseOrderDto>>.Success(purchaseOrders);
    }
}
