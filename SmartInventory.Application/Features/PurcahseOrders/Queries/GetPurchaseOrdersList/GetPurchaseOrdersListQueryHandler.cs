using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrdersList;

public sealed class GetPurchaseOrdersListQueryHandler
    : IRequestHandler<GetPurchaseOrdersListQuery, HandlerResult<IReadOnlyList<PurchaseOrderListDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPurchaseOrdersListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<PurchaseOrderListDto>>> Handle(
        GetPurchaseOrdersListQuery request,
        CancellationToken cancellationToken)
    {
        var purchaseOrders = await _context.PurchaseOrders
            .AsNoTracking()
            .Where(order => order.DeletedAt == null)
            .OrderByDescending(order => order.OrderDate)
            .Select(order => new PurchaseOrderListDto
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                SupplierName = order.Supplier.Name,
                Status = order.Status.ToString(),
                OrderDate = order.OrderDate,
                ItemCount = order.Items.Count(item => item.DeletedAt == null),
                TotalAmount = order.Items
                    .Where(item => item.DeletedAt == null)
                    .Sum(item => item.Quantity * item.UnitAmount)
            })
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<PurchaseOrderListDto>>.Success(purchaseOrders);
    }
}
