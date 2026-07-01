using MediatR;
using SmartInventory.Application.Common;
using SmartInventory.Application.Features.Categories.Commands.CreateCategory;
using SmartInventory.Application.Features.Categories.Queries.GetCategoriesList;

namespace SmartInventory.Presentation.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories")
            .WithTags("Categories");

        group.MapGet("/", GetCategoriesList)
            .WithName("GetCategories")
            .WithSummary("Get all categories");

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new category");

        return app;
    }

    private static async Task<IResult> GetCategoriesList(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesListQuery(), cancellationToken);
        return ToResult(result, value => Results.Ok(value));
    }

    private static async Task<IResult> CreateCategory(
        CreateCategoryRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCategoryCommand(request.CategoryName),
            cancellationToken);

        return ToResult(
            result,
            value => Results.Created($"/api/categories/{value.Id}", value));
    }

    private static IResult ToResult<T>(HandlerResult<T> result, Func<T, IResult> onSuccess)
    {
        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { errors = result.Errors });
        }

        return onSuccess(result.Value!);
    }
}

public sealed record CreateCategoryRequest(string CategoryName);
