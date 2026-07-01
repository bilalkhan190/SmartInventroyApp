using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Categories.Queries.GetCategoriesList;

public sealed record GetCategoriesListQuery : IRequest<HandlerResult<IReadOnlyList<CategoryDto>>>;
