using SmartInventory.Application.Common;

namespace SmartInventory.Presentation.Extensions;

public static class HandlerResultExtensions
{
    public static IResult ToResult<T>(
        this HandlerResult<T> result,
        Func<T, IResult> onSuccess,
        Func<IReadOnlyList<string>, IResult>? onFailure = null)
    {
        if (!result.IsSuccess)
        {
            return onFailure?.Invoke(result.Errors)
                ?? Results.BadRequest(new { errors = result.Errors });
        }

        return onSuccess(result.Value!);
    }
}
