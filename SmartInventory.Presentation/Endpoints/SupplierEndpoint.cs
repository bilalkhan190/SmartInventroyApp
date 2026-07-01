using MediatR;
using SmartInventory.Application.Features.Suppliers.Commands.CreateSupplier;
using SmartInventory.Application.Features.Suppliers.Queries.GetSuppliersList;

namespace SmartInventory.Presentation.Endpoints;

public sealed class SupplierEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/suppliers")
            .WithTags("Suppliers");

        group.MapGet("/", GetSuppliersList)
            .WithName("GetSuppliers")
            .WithSummary("Get all suppliers");

        group.MapPost("/", CreateSupplier)
            .WithName("CreateSupplier")
            .WithSummary("Create a new supplier");
    }

    private static async Task<IResult> GetSuppliersList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSuppliersListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> CreateSupplier(
        CreateSupplierRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateSupplierCommand(
                request.Name,
                request.ContactName,
                request.Email,
                request.Phone,
                request.Address),
            cancellationToken);

        return result.ToResult(
            value => Results.Created($"/api/suppliers/{value.Id}", value));
    }
}

public sealed record CreateSupplierRequest(
    string Name,
    string? ContactName,
    string? Email,
    string? Phone,
    string? Address);
