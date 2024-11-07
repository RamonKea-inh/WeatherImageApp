using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Web;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Domain.Interfaces;

public class ImageFunction : BaseFunction
{
    private readonly IWeatherFacade _weatherFacade;
    private readonly UnsplashClient _unsplashClient;

    public ImageFunction(IWeatherFacade weatherFacade, UnsplashClient unsplashClient, ILogger<ImageFunction> logger) : base(logger)
    {
        _weatherFacade = weatherFacade;
        _unsplashClient = unsplashClient;
    }


    [Function("GetImages")]
    public async Task<HttpResponseData> GetImages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "images")] HttpRequestData req)
    {
        return await ExecuteAsync(req, async () =>
        {
            var queryParameters = HttpUtility.ParseQueryString(req.Url.Query);
            var query = queryParameters["query"] ?? "nature";
            var count = int.TryParse(queryParameters["count"], out var parsedCount) ? parsedCount : 10;

            var imageResult = await _unsplashClient.GetRandomNaturePhotosAsync(query, count);

            if (!imageResult.IsSuccess || imageResult.Data == null)
            {
                return Result<List<string>>.Failure("Failed to retrieve images from Unsplash");
            }

            var imageUrls = imageResult.Data.Select(img => img.Urls.Regular).ToList();
            return Result<List<string>>.Success(imageUrls);
        }, "GetImages");
    }
}
