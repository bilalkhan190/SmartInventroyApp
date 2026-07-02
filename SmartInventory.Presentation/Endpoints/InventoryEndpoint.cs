using MediatR;
using SmartInventory.Application.Features.Inventory.Queries.GetInventoriesList;

namespace SmartInventory.Presentation.Endpoints;

public sealed class InventoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory")
            .WithTags("Inventory");

        group.MapGet("/", GetInventoriesList)
            .WithName("GetInventories")
            .WithSummary("Get current stock for all products");
    }

    private static async Task<IResult> GetInventoriesList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInventoriesListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }
}
