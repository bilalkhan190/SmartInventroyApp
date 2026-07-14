using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Application.Features.PurcahseOrders;
using SmartInventory.Domain.Common;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.RequestPurchaseOrderApproval;

public sealed class RequestPurchaseOrderApprovalHandler
    : IRequestHandler<RequestPurchaseOrderApprovalCommand, HandlerResult<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;

    public RequestPurchaseOrderApprovalHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<PurchaseOrderDto>> Handle(
        RequestPurchaseOrderApprovalCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .FirstOrDefaultAsync(
                order => order.Id == request.Id && order.DeletedAt == null,
                cancellationToken);

        if (purchaseOrder is null)
        {
            return HandlerResult<PurchaseOrderDto>.Failure("Purchase order not found.");
        }

        try
        {
            purchaseOrder.SubmitForApproval();
        }
        catch (DomainException ex)
        {
            return HandlerResult<PurchaseOrderDto>.Failure(ex.Message);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return HandlerResult<PurchaseOrderDto>.Success(
            await MapToDtoAsync(purchaseOrder.Id, cancellationToken));
    }

    private async Task<PurchaseOrderDto> MapToDtoAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.PurchaseOrders
            .AsNoTracking()
            .Where(order => order.Id == id)
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
            .FirstAsync(cancellationToken);
}
