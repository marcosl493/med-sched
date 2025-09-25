using FluentResults;

namespace WebApi.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if(result.IsSuccess && result.ValueOrDefault is null)
            return Results.NotFound();

        if (result.ValueOrDefault is not null)
            return Results.Ok(result.Value);

        var firstError = result.Errors[0];

        return firstError switch
        {
            Error e when e.Metadata.ContainsKey("StatusCode") =>
                Results.Problem(
                    detail: e.Message,
                    statusCode: (int)e.Metadata["StatusCode"]
                ),

            _ => Results.BadRequest(new
            {
                Errors = result.Errors.Select(e => e.Message).ToArray()
            })
        };
    }
    public static IResult ToCreatedAtRouteResult<T>(
    this Result<T> result,
    string routeName,
    Func<T, object> routeValuesFunc)
    {
        if (result.IsSuccess)
        {
            var value = result.Value;
            var routeValues = routeValuesFunc(value);
            return Results.CreatedAtRoute(routeName, routeValues, value);
        }

        var firstError = result.Errors[0];
        return firstError switch
        {
            { } e when e.Metadata.ContainsKey("StatusCode") =>
                Results.Problem(detail: e.Message, statusCode: (int)e.Metadata["StatusCode"]),
            _ => Results.BadRequest(new { Errors = result.Errors.Select(e => e.Message) })
        };
    }
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.NoContent();

        var firstError = result.Errors[0];

        return firstError switch
        {
            Error e when e.Metadata.ContainsKey("StatusCode") =>
                Results.Problem(
                    detail: e.Message,
                    statusCode: (int)e.Metadata["StatusCode"]
                ),
            _ => Results.BadRequest(new
            {
                Errors = result.Errors.Select(e => e.Message).ToArray()
            })
        };
    }
}
