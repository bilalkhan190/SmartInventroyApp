using MediatR;
using SmartInventory.Application.Features.PurcahseOrders.Command.ApprovePurchaseOrder;
using SmartInventory.Application.Features.PurcahseOrders.Command.CreatePurcahseOrder;
using SmartInventory.Application.Features.PurcahseOrders.Command.RejectPurchaseOrder;
using SmartInventory.Application.Features.PurcahseOrders.Command.RequestPurchaseOrderApproval;
using SmartInventory.Application.Features.PurcahseOrders.Command.UpdatePurchaseOrder;
using SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderById;
using SmartInventory.Application.Features.PurcahseOrders.Queries.GetPurchaseOrderForApproval;
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

        group.MapGet("/for-approval", GetPurchaseOrdersForApproval)
            .WithName("GetPurchaseOrdersForApproval")
            .WithSummary("Get purchase orders pending approval");

        group.MapGet("/{id:guid}", GetPurchaseOrderById)
            .WithName("GetPurchaseOrderById")
            .WithSummary("Get purchase order by id");

        group.MapPost("/", CreatePurchaseOrder)
            .WithName("CreatePurchaseOrder")
            .WithSummary("Create a new purchase order");

        group.MapPut("/{id:guid}", UpdatePurchaseOrder)
            .WithName("UpdatePurchaseOrder")
            .WithSummary("Update a pending purchase order");

        group.MapPost("/{id:guid}/approve", ApprovePurchaseOrder)
            .WithName("ApprovePurchaseOrder")
            .WithSummary("Approve a pending purchase order");

        group.MapPost("/{id:guid}/reject", RejectPurchaseOrder)
            .WithName("RejectPurchaseOrder")
            .WithSummary("Reject a pending purchase order");

        group.MapPost("/{id:guid}/request-approval", RequestPurchaseOrderApproval)
            .WithName("RequestPurchaseOrderApproval")
            .WithSummary("Submit a purchase order for approval");
    }

    private static async Task<IResult> GetPurchaseOrdersList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrdersListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> GetPurchaseOrdersForApproval(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrderForApprovalQuery(), cancellationToken);
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

    private static async Task<IResult> UpdatePurchaseOrder(
        Guid id,
        UpdatePurchaseOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePurchaseOrderCommand(
                id,
                request.SupplierId,
                request.Products,
                request.Notes),
            cancellationToken);

        return result.ToResult(
            value => Results.Ok(value),
            errors => errors.Contains("Purchase order not found.")
                ? Results.NotFound(new { errors })
                : Results.BadRequest(new { errors }));
    }

    private static async Task<IResult> ApprovePurchaseOrder(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ApprovePurchaseOrderCommand(id), cancellationToken);
        return result.ToResult(
            value => Results.Ok(value),
            errors => errors.Contains("Purchase order not found.")
                ? Results.NotFound(new { errors })
                : Results.BadRequest(new { errors }));
    }

    private static async Task<IResult> RejectPurchaseOrder(
        Guid id,
        RejectPurchaseOrderRequest? request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RejectPurchaseOrderCommand(id, request?.Reason),
            cancellationToken);

        return result.ToResult(
            value => Results.Ok(value),
            errors => errors.Contains("Purchase order not found.")
                ? Results.NotFound(new { errors })
                : Results.BadRequest(new { errors }));
    }

    private static async Task<IResult> RequestPurchaseOrderApproval(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RequestPurchaseOrderApprovalCommand(id), cancellationToken);
        return result.ToResult(
            value => Results.Ok(value),
            errors => errors.Contains("Purchase order not found.")
                ? Results.NotFound(new { errors })
                : Results.BadRequest(new { errors }));
    }
}

public sealed record CreatePurchaseOrderRequest(
    Guid SupplierId,
    IReadOnlyList<PurchaseOrderProductInfo> Products,
    string? Notes = null);

public sealed record UpdatePurchaseOrderRequest(
    Guid SupplierId,
    IReadOnlyList<PurchaseOrderProductInfo> Products,
    string? Notes = null);

public sealed record RejectPurchaseOrderRequest(string? Reason = null);
