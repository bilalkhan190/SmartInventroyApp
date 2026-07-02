using MediatR;
using SmartInventory.Application.Features.PurcahseOrders.Command.CreatePurcahseOrder;
using SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderById;
using SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrdersList;

namespace SmartInventory.Presentation.Endpoints;

public sealed class PurchaseOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/purchase-orders")
            .WithTags("Purchase Orders");

        group.MapGet("/", GetPurchaseOrdersList)
            .WithName("GetPurchaseOrders")
            .WithSummary("Get all purchase orders");

        group.MapGet("/{id:guid}", GetPurchaseOrderById)
            .WithName("GetPurchaseOrderById")
            .WithSummary("Get purchase order by id");

        group.MapPost("/", CreatePurchaseOrder)
            .WithName("CreatePurchaseOrder")
            .WithSummary("Create a new purchase order");
    }

    private static async Task<IResult> GetPurchaseOrdersList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrdersListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> GetPurchaseOrderById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> CreatePurchaseOrder(
        CreatePurchaseOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePurchaseOrderCommand(
                request.SupplierId,
                request.Products,
                request.Notes),
            cancellationToken);

        return result.ToResult(
            value => Results.Created($"/api/purchase-orders/{value.Id}", value));
    }
}

public sealed record CreatePurchaseOrderRequest(
    Guid SupplierId,
    IReadOnlyList<PurchaseOrderProductInfo> Products,
    string? Notes = null);
