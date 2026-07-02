using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, HandlerResult<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<UpdateProductCommand> _validator;

    public UpdateProductCommandHandler(
        IApplicationDbContext context,
        IValidator<UpdateProductCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<HandlerResult<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<ProductDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id && p.DeletedAt == null,
                cancellationToken);

        if (product is null)
        {
            return HandlerResult<ProductDto>.Failure("Product not found.");
        }

        var categoryExists = await _context.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId && category.DeletedAt == null,
                cancellationToken);

        if (!categoryExists)
        {
            return HandlerResult<ProductDto>.Failure("Category not found.");
        }

        var normalizedSku = request.Sku.Trim();
        var skuExists = await _context.Products
            .AnyAsync(
                p => p.Sku == normalizedSku && p.Id != request.Id && p.DeletedAt == null,
                cancellationToken);

        if (skuExists)
        {
            return HandlerResult<ProductDto>.Failure("Product with this SKU already exists.");
        }

        product.Name = request.Name.Trim();
        product.Sku = normalizedSku;
        product.Description = NormalizeOptional(request.Description);
        product.CategoryId = request.CategoryId;
        product.Quantity = request.Quantity;
        product.ReorderLevel = request.ReorderLevel;
        product.MarkUpdated();

        var inventory = await _context.Inventories
            .FirstOrDefaultAsync(
                entry => entry.ProductId == product.Id && entry.DeletedAt == null,
                cancellationToken);

        if (inventory is null)
        {
            _context.Inventories.Add(new Domain.Entities.Inventory
            {
                ProductId = product.Id,
                CurrentStockQuantity = request.Quantity
            });
        }
        else
        {
            inventory.CurrentStockQuantity = request.Quantity;
            inventory.MarkUpdated();
        }

        await _context.SaveChangesAsync(cancellationToken);

        var dto = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == product.Id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Sku = p.Sku,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                Quantity = p.ProductInventory != null
                    ? p.ProductInventory.CurrentStockQuantity
                    : p.Quantity,
                ReorderLevel = p.ReorderLevel,
                CreatedAt = p.CreatedAt
            })
            .FirstAsync(cancellationToken);

        return HandlerResult<ProductDto>.Success(dto);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
