using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

public abstract class BaseFunction
{
    protected readonly ILogger _logger;

    protected BaseFunction(ILogger logger)
    {
        _logger = logger;
    }

    protected async Task<HttpResponseData> ExecuteAsync<T>(
        HttpRequestData req,
        Func<Task<Result<T>>> action,
        string operationName)
    {
        try
        {
            _logger.LogInformation("Processing {Operation} request", operationName);

            var result = await action();
            var response = req.CreateResponse();

            if (result.IsSuccess)
            {
                response.StatusCode = HttpStatusCode.OK;
                await response.WriteAsJsonAsync(result.Data);
            }
            else
            {
                response.StatusCode = HttpStatusCode.BadRequest;
                await response.WriteAsJsonAsync(new
                {
                    error = result.Error,
                    code = result.ErrorCode
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {Operation} request", operationName);

            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred",
                code = "INTERNAL_ERROR",
                details = ex.Message // Only in development
            });

            return response;
        }
    }
}