using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Application.Features.PurcahseOrders;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.UpdatePurchaseOrder;

public sealed class UpdatePurchaseOrderHandler
    : IRequestHandler<UpdatePurchaseOrderCommand, HandlerResult<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<UpdatePurchaseOrderCommand> _validator;

    public UpdatePurchaseOrderHandler(
        IApplicationDbContext context,
        IValidator<UpdatePurchaseOrderCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<HandlerResult<PurchaseOrderDto>> Handle(
        UpdatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<PurchaseOrderDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        var purchaseOrder = await _context.PurchaseOrders
            .Include(order => order.Items.Where(item => item.DeletedAt == null))
            .FirstOrDefaultAsync(
                order => order.Id == request.Id && order.DeletedAt == null,
                cancellationToken);

        if (purchaseOrder is null)
        {
            return HandlerResult<PurchaseOrderDto>.Failure("Purchase order not found.");
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.Pending &&
            purchaseOrder.Status != PurchaseOrderStatus.Revised)
        {
            return HandlerResult<PurchaseOrderDto>.Failure(
                "Only pending or revised purchase orders can be edited.");
        }

        var supplierExists = await _context.Suppliers
            .AnyAsync(
                supplier => supplier.Id == request.SupplierId && supplier.DeletedAt == null,
                cancellationToken);

        if (!supplierExists)
        {
            return HandlerResult<PurchaseOrderDto>.Failure("Supplier not found.");
        }

        var productIds = request.Products.Select(product => product.ProductId).ToList();
        var existingProducts = await _context.Products
            .AsNoTracking()
            .Where(product => productIds.Contains(product.Id) && product.DeletedAt == null)
            .Select(product => new { product.Id, product.Name, product.Sku })
            .ToListAsync(cancellationToken);

        if (existingProducts.Count != productIds.Count)
        {
            return HandlerResult<PurchaseOrderDto>.Failure("One or more products were not found.");
        }

        var productsById = existingProducts.ToDictionary(product => product.Id);

        purchaseOrder.SupplierId = request.SupplierId;
        purchaseOrder.Notes = NormalizeOptional(request.Notes);
        purchaseOrder.MarkUpdated();

        foreach (var existingItem in purchaseOrder.Items)
        {
            existingItem.MarkDeleted();
        }

        foreach (var item in request.Products)
        {
            purchaseOrder.Items.Add(new PurchaseOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitAmount = item.UnitAmount
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return HandlerResult<PurchaseOrderDto>.Success(new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            OrderNo = purchaseOrder.OrderNo,
            SupplierId = purchaseOrder.SupplierId,
            SupplierName = await _context.Suppliers
                .AsNoTracking()
                .Where(supplier => supplier.Id == purchaseOrder.SupplierId)
                .Select(supplier => supplier.Name)
                .FirstAsync(cancellationToken),
            Status = purchaseOrder.Status.ToString(),
            OrderDate = purchaseOrder.OrderDate,
            Notes = purchaseOrder.Notes,
            Items = purchaseOrder.Items
                .Where(item => item.DeletedAt == null)
                .Select(item => new PurchaseOrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = productsById[item.ProductId].Name,
                    Sku = productsById[item.ProductId].Sku,
                    Quantity = item.Quantity,
                    UnitAmount = item.UnitAmount
                })
                .ToList()
        });
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
