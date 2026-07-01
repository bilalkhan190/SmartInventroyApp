using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Common;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Features.Categories.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, HandlerResult<CategoryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IValidator<CreateCategoryCommand> _validator;

    public CreateCategoryCommandHandler(
        IApplicationDbContext context,
        IValidator<CreateCategoryCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<HandlerResult<CategoryDto>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return HandlerResult<CategoryDto>.Failure(
                validation.Errors.Select(error => error.ErrorMessage).ToArray());
        }

        var normalizedName = request.CategoryName.Trim();
        var exists = await _context.Categories
            .AnyAsync(
                category => category.CategoryName == normalizedName && category.DeletedAt == null,
                cancellationToken);

        if (exists)
        {
            return HandlerResult<CategoryDto>.Failure("Category with this name already exists.");
        }

        var category = new Category
        {
            CategoryName = normalizedName
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return HandlerResult<CategoryDto>.Success(new CategoryDto
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            CreatedAt = category.CreatedAt
        });
    }
}
