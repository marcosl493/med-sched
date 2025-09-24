using FluentResults;

namespace WebApi.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
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
