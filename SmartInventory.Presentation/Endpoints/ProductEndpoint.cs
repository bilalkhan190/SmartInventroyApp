using MediatR;
using SmartInventory.Application.Features.Products.Commands.CreateProduct;
using SmartInventory.Application.Features.Products.Commands.DeleteProduct;
using SmartInventory.Application.Features.Products.Commands.UpdateProduct;
using SmartInventory.Application.Features.Products.Queries.GetProductById;
using SmartInventory.Application.Features.Products.Queries.GetProductsList;

namespace SmartInventory.Presentation.Endpoints;

public sealed class ProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", GetProductsList)
            .WithName("GetProducts")
            .WithSummary("Get all products");

        group.MapGet("/{id:guid}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get product by id");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product");

        group.MapPut("/{id:guid}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product");

        group.MapDelete("/{id:guid}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product");
    }

    private static async Task<IResult> GetProductsList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductsListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> GetProductById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
        return result.ToResult(
            value => Results.Ok(value),
            errors => Results.NotFound(new { errors }));
    }

    private static async Task<IResult> CreateProduct(
        CreateProductRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateProductCommand(
                request.Name,
                request.Sku,
                request.Description,
                request.CategoryId,
                request.Quantity,
                request.ReorderLevel),
            cancellationToken);

        return result.ToResult(
            value => Results.Created($"/api/products/{value.Id}", value));
    }

    private static async Task<IResult> UpdateProduct(
        Guid id,
        UpdateProductRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProductCommand(
                id,
                request.Name,
                request.Sku,
                request.Description,
                request.CategoryId,
                request.Quantity,
                request.ReorderLevel),
            cancellationToken);

        return result.ToResult(
            value => Results.Ok(value),
            errors => errors.Contains("Product not found.")
                ? Results.NotFound(new { errors })
                : Results.BadRequest(new { errors }));
    }

    private static async Task<IResult> DeleteProduct(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);
        return result.ToResult(
            _ => Results.NoContent(),
            errors => errors.Contains("Product not found.")
                ? Results.NotFound(new { errors })
                : Results.BadRequest(new { errors }));
    }
}

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    string? Description,
    Guid CategoryId,
    int Quantity,
    int ReorderLevel);

public sealed record UpdateProductRequest(
    string Name,
    string Sku,
    string? Description,
    Guid CategoryId,
    int Quantity,
    int ReorderLevel);
