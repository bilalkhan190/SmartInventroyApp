using MediatR;
using SmartInventory.Application.Common;

namespace SmartInventory.Application.Features.Inventory.Queries.GetInventoriesList;

public sealed record GetInventoriesListQuery
    : IRequest<HandlerResult<IReadOnlyList<InventoryListDto>>>;
