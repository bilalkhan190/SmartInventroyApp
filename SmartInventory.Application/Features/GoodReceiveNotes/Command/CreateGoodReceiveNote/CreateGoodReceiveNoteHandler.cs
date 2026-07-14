using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Application.Features.GoodReceiveNotes;
using SmartInventory.Domain.Entities;
using SmartInventory.Domain.Enums;

namespace SmartInventory.Application.Features.GoodReceiveNotes.Command.CreateGoodReceiveNote;

public sealed class CreateGoodReceiveNoteHandler
    : IRequestHandler<CreateGoodReceiveNoteCommand, HandlerResult<GoodReceiveNoteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IDocumentNumberGenerator _documentNumberGenerator;
    private readonly IValidator<CreateGoodReceiveNoteCommand> _validator;

    public CreateGoodReceiveNoteHandler(
        IApplicationDbContext context,
        IDocumentNumberGenerator documentNumberGenerator,
        IValidator<CreateGoodReceiveNoteCommand> validator)
    {
        _context = context;
        _documentNumberGenerator = documentNumberGenerator;
        _validator = validator;
    }

    public async Task<HandlerResult<GoodReceiveNoteDto>> Handle(
        CreateGoodReceiveNoteCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<GoodReceiveNoteDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        var purchaseOrder = await _context.PurchaseOrders
            .Include(order => order.Items.Where(item => item.DeletedAt == null))
            .FirstOrDefaultAsync(
                order => order.Id == request.PurchaseOrderId && order.DeletedAt == null,
                cancellationToken);

        if (purchaseOrder is null)
        {
            return HandlerResult<GoodReceiveNoteDto>.Failure("Purchase order not found.");
        }

        if (purchaseOrder.Status != PurchaseOrderStatus.Approved)
        {
            return HandlerResult<GoodReceiveNoteDto>.Failure(
                "Goods can only be received against approved purchase orders.");
        }

        var poItemsByProductId = purchaseOrder.Items
            .ToDictionary(item => item.ProductId);

        var requestProductIds = request.Items.Select(item => item.ProductId).ToList();
        var missingProducts = requestProductIds
            .Where(productId => !poItemsByProductId.ContainsKey(productId))
            .ToList();

        if (missingProducts.Count > 0)
        {
            return HandlerResult<GoodReceiveNoteDto>.Failure(
                "One or more products are not part of the selected purchase order.");
        }

        var products = await _context.Products
            .Where(product => requestProductIds.Contains(product.Id) && product.DeletedAt == null)
            .ToListAsync(cancellationToken);

        if (products.Count != requestProductIds.Count)
        {
            return HandlerResult<GoodReceiveNoteDto>.Failure("One or more products were not found.");
        }

        var inventories = await _context.Inventories
            .Where(inventory => requestProductIds.Contains(inventory.ProductId) && inventory.DeletedAt == null)
            .ToListAsync(cancellationToken);

        if (inventories.Count != requestProductIds.Count)
        {
            return HandlerResult<GoodReceiveNoteDto>.Failure(
                "Inventory record not found for one or more products.");
        }

        var productsById = products.ToDictionary(product => product.Id);
        var inventoriesByProductId = inventories.ToDictionary(inventory => inventory.ProductId);
        var grnNo = await _documentNumberGenerator.NextGrnNoAsync(cancellationToken);

        var goodReceiveNote = new GoodReceiveNote
        {
            GrnNo = grnNo,
            SupplierId = purchaseOrder.SupplierId,
            PurchaseOrderId = purchaseOrder.Id,
            ReceiveDate = request.ReceiveDate ?? DateTime.UtcNow,
            ReceivedBy = Guid.Empty
        };

        foreach (var item in request.Items)
        {
            var poItem = poItemsByProductId[item.ProductId];

            goodReceiveNote.GoodReceiveNoteItems.Add(new GoodReceiveNoteItem
            {
                ProductId = item.ProductId,
                OrderedQuantity = poItem.Quantity,
                ReceivedQuantity = item.ReceivedQuantity,
                AcceptedQuantity = item.AcceptedQuantity,
                UnitPrice = poItem.UnitAmount,
                BatchNo = NormalizeOptional(item.BatchNo),
                ExpiredAt = item.ExpiredAt,
                Remarks = NormalizeOptional(item.Remarks)
            });

            if (item.AcceptedQuantity > 0)
            {
                var inventory = inventoriesByProductId[item.ProductId];
                inventory.CurrentStockQuantity += item.AcceptedQuantity;
                inventory.MarkUpdated();
            }
        }

        _context.GoodReceiveNotes.Add(goodReceiveNote);
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var item in request.Items.Where(line => line.AcceptedQuantity > 0))
        {
            _context.StockMovements.Add(new StockMovement
            {
                ProductId = item.ProductId,
                Type = StockMovementType.In,
                ReferenceId = goodReceiveNote.Id,
                StockReferenceType = StockReferenceType.GoodsReceipt,
                ReferenceNo = grnNo,
                Quantity = item.AcceptedQuantity,
                Reason = $"Goods received via {grnNo}"
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var supplierName = await _context.Suppliers
            .AsNoTracking()
            .Where(supplier => supplier.Id == purchaseOrder.SupplierId)
            .Select(supplier => supplier.Name)
            .FirstAsync(cancellationToken);

        return HandlerResult<GoodReceiveNoteDto>.Success(new GoodReceiveNoteDto
        {
            Id = goodReceiveNote.Id,
            GrnNo = goodReceiveNote.GrnNo,
            SupplierId = goodReceiveNote.SupplierId,
            SupplierName = supplierName,
            PurchaseOrderId = goodReceiveNote.PurchaseOrderId,
            PurchaseOrderNo = purchaseOrder.OrderNo,
            ReceiveDate = goodReceiveNote.ReceiveDate,
            Items = goodReceiveNote.GoodReceiveNoteItems.Select(noteItem => new GoodReceiveNoteItemDto
            {
                Id = noteItem.Id,
                ProductId = noteItem.ProductId,
                ProductName = productsById[noteItem.ProductId].Name,
                Sku = productsById[noteItem.ProductId].Sku,
                OrderedQuantity = noteItem.OrderedQuantity,
                ReceivedQuantity = noteItem.ReceivedQuantity,
                AcceptedQuantity = noteItem.AcceptedQuantity,
                UnitPrice = noteItem.UnitPrice,
                BatchNo = noteItem.BatchNo,
                ExpiredAt = noteItem.ExpiredAt,
                Remarks = noteItem.Remarks
            }).ToList()
        });
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
