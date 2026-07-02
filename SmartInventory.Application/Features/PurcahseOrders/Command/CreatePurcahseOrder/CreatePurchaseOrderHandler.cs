using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Application.Features.PurcahseOrders;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.Features.PurcahseOrders.Command.CreatePurcahseOrder;

public sealed class CreatePurchaseOrderCommandHandler
    : IRequestHandler<CreatePurchaseOrderCommand, HandlerResult<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly IValidator<CreatePurchaseOrderCommand> _validator;

    public CreatePurchaseOrderCommandHandler(
        IApplicationDbContext context,
        IDocumentNumberGenerator documentNumberGenerator,
        IValidator<CreatePurchaseOrderCommand> validator)
    {
        _context = context;
        _documentNumberGenerator = documentNumberGenerator;
        _validator = validator;
    }

    public async Task<HandlerResult<PurchaseOrderDto>> Handle(
        CreatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<PurchaseOrderDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
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

        var orderNo = await _documentNumberGenerator.NextPurchaseOrderNoAsync(cancellationToken);

        var purchaseOrder = new PurchaseOrder
        {
            OrderNo = orderNo,
            SupplierId = request.SupplierId,
            Status = PurchaseOrderStatus.Pending,
            OrderDate = DateTime.UtcNow,
            Notes = NormalizeOptional(request.Notes)
        };

        foreach (var item in request.Products)
        {
            purchaseOrder.Items.Add(new PurchaseOrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitAmount = item.UnitAmount
            });
        }

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync(cancellationToken);

        var supplierName = await _context.Suppliers
            .AsNoTracking()
            .Where(supplier => supplier.Id == request.SupplierId)
            .Select(supplier => supplier.Name)
            .FirstAsync(cancellationToken);

        return HandlerResult<PurchaseOrderDto>.Success(new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            OrderNo = purchaseOrder.OrderNo,
            SupplierId = purchaseOrder.SupplierId,
            SupplierName = supplierName,
            Status = purchaseOrder.Status.ToString(),
            OrderDate = purchaseOrder.OrderDate,
            Notes = purchaseOrder.Notes,
            Items = purchaseOrder.Items.Select(item => new PurchaseOrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = productsById[item.ProductId].Name,
                Sku = productsById[item.ProductId].Sku,
                Quantity = item.Quantity,
                UnitAmount = item.UnitAmount
            }).ToList()
        });
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
