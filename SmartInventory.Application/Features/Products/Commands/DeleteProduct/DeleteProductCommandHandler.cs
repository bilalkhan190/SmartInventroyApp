using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;

namespace SmartInventory.Application.Features.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, HandlerResult<bool>>
{
    private readonly IApplicationDbContext _context;

    public DeleteProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HandlerResult<bool>> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                p => p.Id == request.Id && p.DeletedAt == null,
                cancellationToken);

        if (product is null)
        {
            return HandlerResult<bool>.Failure("Product not found.");
        }

        product.MarkDeleted();
        await _context.SaveChangesAsync(cancellationToken);

        return HandlerResult<bool>.Success(true);
    }
}
