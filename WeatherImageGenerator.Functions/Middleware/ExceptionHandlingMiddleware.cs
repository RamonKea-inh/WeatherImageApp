using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using WeatherImageGenerator.Domain.Exceptions;
using Microsoft.Azure.Functions.Worker.Http;

namespace WeatherImageGenerator.Functions.Middleware
{
    public class ExceptionHandlingMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                var httpRequestData = await context.GetHttpRequestDataAsync();
                if (httpRequestData != null)
                {
                    var response = httpRequestData.CreateResponse();

                    (HttpStatusCode statusCode, object errorResponse) = ex switch
                    {
                        WeatherServiceException weatherEx => (HttpStatusCode.BadGateway, new
                        {
                            error = weatherEx.Message,
                            code = weatherEx.ErrorCode
                        }),

                        ArgumentException _ => (HttpStatusCode.BadRequest, new
                        {
                            error = "Invalid request parameters",
                            code = "INVALID_REQUEST"
                        }),

                        _ => (HttpStatusCode.InternalServerError, new
                        {
                            error = "An unexpected error occurred",
                            code = "INTERNAL_ERROR"
                        })
                    };

                    response.StatusCode = statusCode;
                    await response.WriteAsJsonAsync(errorResponse);

                    context.GetInvocationResult().Value = response;
                }
            }
        }
    }
}