using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Categories.Queries.GetCategoriesList;

public sealed class GetCategoriesListQueryHandler
    : IRequestHandler<GetCategoriesListQuery, HandlerResult<IReadOnlyList<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoriesListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesListQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(category => category.DeletedAt == null)
            .OrderBy(category => category.CategoryName)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                CreatedAt = category.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return HandlerResult<IReadOnlyList<CategoryDto>>.Success(categories);
    }
}
