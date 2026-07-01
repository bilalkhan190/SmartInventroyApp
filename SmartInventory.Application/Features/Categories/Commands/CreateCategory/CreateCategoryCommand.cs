using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(string CategoryName) : IRequest<HandlerResult<CategoryDto>>;
