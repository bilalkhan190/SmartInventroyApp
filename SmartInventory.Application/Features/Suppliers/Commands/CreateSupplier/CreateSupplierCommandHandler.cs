using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Features.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierCommandHandler
    : IRequestHandler<CreateSupplierCommand, HandlerResult<SupplierDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateSupplierCommand> _validator;

    public CreateSupplierCommandHandler(
        IApplicationDbContext context,
        IValidator<CreateSupplierCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<HandlerResult<SupplierDto>> Handle(
        CreateSupplierCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<SupplierDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        var normalizedName = request.Name.Trim();
        var exists = await _context.Suppliers
            .AnyAsync(
                supplier => supplier.Name == normalizedName && supplier.DeletedAt == null,
                cancellationToken);

        if (exists)
        {
            return HandlerResult<SupplierDto>.Failure("Supplier with this name already exists.");
        }

        var supplier = new Supplier
        {
            Name = normalizedName,
            ContactName = NormalizeOptional(request.ContactName),
            Email = NormalizeOptional(request.Email),
            Phone = NormalizeOptional(request.Phone),
            Address = NormalizeOptional(request.Address)
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return HandlerResult<SupplierDto>.Success(MapToDto(supplier));
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SupplierDto MapToDto(Supplier supplier) => new()
    {
        Id = supplier.Id,
        Name = supplier.Name,
        ContactName = supplier.ContactName,
        Email = supplier.Email,
        Phone = supplier.Phone,
        Address = supplier.Address,
        CreatedAt = supplier.CreatedAt
    };
}
