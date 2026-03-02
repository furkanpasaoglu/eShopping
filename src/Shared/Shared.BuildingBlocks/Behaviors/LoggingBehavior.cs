using MediatR;
using Microsoft.Extensions.Logging;
using Shared.BuildingBlocks.Results;

namespace Shared.BuildingBlocks.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            logger.LogInformation("Handled {RequestName} successfully", requestName);
        }
        else
        {
            logger.LogWarning(
                "Request {RequestName} failed: [{ErrorCode}] {ErrorDescription}",
                requestName,
                response.Error.Code,
                response.Error.Description);
        }

        return response;
    }
}
