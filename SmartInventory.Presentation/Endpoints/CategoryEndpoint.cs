using MediatR;
using SmartInventory.Application.Features.Categories.Commands.CreateCategory;
using SmartInventory.Application.Features.Categories.Queries.GetCategoriesList;

namespace SmartInventory.Presentation.Endpoints;

public sealed class CategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories");

        group.MapGet("/", GetCategoriesList)
            .WithName("GetCategories")
            .WithSummary("Get all categories");

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new category");
    }

    private static async Task<IResult> GetCategoriesList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesListQuery(), cancellationToken);
        return result.ToResult(value => Results.Ok(value));
    }

    private static async Task<IResult> CreateCategory(
        CreateCategoryRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCategoryCommand(request.CategoryName),
            cancellationToken);

        return result.ToResult(
            value => Results.Created($"/api/categories/{value.Id}", value));
    }
}

public sealed record CreateCategoryRequest(string CategoryName);
