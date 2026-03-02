using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.BuildingBlocks.Results;
using HttpResults = Microsoft.AspNetCore.Http.Results;

namespace Shared.BuildingBlocks.Extensions;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? HttpResults.Ok(result.Value)
            : result.Error.ToHttpResult();

    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess
            ? HttpResults.Ok()
            : result.Error.ToHttpResult();

    private static IResult ToHttpResult(this Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => HttpResults.NotFound(
                new ProblemDetails { Title = error.Code, Detail = error.Description }),
            ErrorType.Validation => HttpResults.UnprocessableEntity(
                new ProblemDetails
                {
                    Title = error.Code,
                    Detail = error.Description,
                    Status = StatusCodes.Status422UnprocessableEntity
                }),
            ErrorType.Conflict => HttpResults.Conflict(
                new ProblemDetails { Title = error.Code, Detail = error.Description }),
            ErrorType.Unauthorized => HttpResults.Unauthorized(),
            ErrorType.Forbidden => HttpResults.Forbid(),
            _ => HttpResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: error.Code,
                detail: error.Description)
        };
}
