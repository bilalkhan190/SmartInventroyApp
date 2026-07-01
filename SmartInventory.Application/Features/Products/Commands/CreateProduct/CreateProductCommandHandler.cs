using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, HandlerResult<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateProductCommand> _validator;

    public CreateProductCommandHandler(
        IApplicationDbContext context,
        IValidator<CreateProductCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<HandlerResult<ProductDto>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<ProductDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        var normalizedSku = request.Sku.Trim();
        var categoryExists = await _context.Categories
            .AnyAsync(
                category => category.Id == request.CategoryId && category.DeletedAt == null,
                cancellationToken);

        if (!categoryExists)
        {
            return HandlerResult<ProductDto>.Failure("Category not found.");
        }

        var skuExists = await _context.Products
            .AnyAsync(
                product => product.Sku == normalizedSku && product.DeletedAt == null,
                cancellationToken);

        if (skuExists)
        {
            return HandlerResult<ProductDto>.Failure("Product with this SKU already exists.");
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Sku = normalizedSku,
            Description = NormalizeOptional(request.Description),
            CategoryId = request.CategoryId,
            Quantity = request.Quantity,
            ReorderLevel = request.ReorderLevel
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return HandlerResult<ProductDto>.Success(await MapToDtoAsync(product.Id, cancellationToken));
    }

    private async Task<ProductDto> MapToDtoAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product => product.Id == productId)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                Quantity = product.Quantity,
                ReorderLevel = product.ReorderLevel,
                CreatedAt = product.CreatedAt
            })
            .FirstAsync(cancellationToken);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
