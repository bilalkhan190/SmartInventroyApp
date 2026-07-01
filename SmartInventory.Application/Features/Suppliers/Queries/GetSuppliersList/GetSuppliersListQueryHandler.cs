using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Suppliers.Queries.GetSuppliersList;

public sealed class GetSuppliersListQueryHandler
    : IRequestHandler<GetSuppliersListQuery, HandlerResult<IReadOnlyList<SupplierDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetSuppliersListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<SupplierDto>>> Handle(
        GetSuppliersListQuery request,
        CancellationToken cancellationToken)
    {
        var suppliers = await _context.Suppliers
            .AsNoTracking()
            .Where(supplier => supplier.DeletedAt == null)
            .OrderBy(supplier => supplier.Name)
            .Select(supplier => new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactName = supplier.ContactName,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                CreatedAt = supplier.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<SupplierDto>>.Success(suppliers);
    }
}
