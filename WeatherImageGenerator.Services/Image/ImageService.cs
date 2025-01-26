using Microsoft.Extensions.Logging;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Domain.Interfaces;

namespace WeatherImageGenerator.Services.Image
{
    public class ImageService : IImageService
    {
        private readonly UnsplashClient _unsplashClient;
        private readonly ILogger<ImageService> _logger;

        public ImageService(UnsplashClient unsplashClient, ILogger<ImageService> logger)
        {
            _unsplashClient = unsplashClient;
            _logger = logger;
        }

        public async Task<Result<List<string>>> GetRandomNatureImagesUrlAsync(string query = "nature", int count = 1)
        {
            try
            {
                var result = await _unsplashClient.GetRandomNaturePhotosAsync(query, count);
                if (result.IsSuccess && result.Data != null)
                {
                    var imageUrls = result.Data.Select(img => img.Urls.Regular).ToList();
                    return Result<List<string>>.Success(imageUrls);
                }
                else
                {
                    _logger.LogError($"Failed to get random images: {result.Error}");
                    return Result<List<string>>.Failure(result.Error ?? "Unknown error", result.ErrorCode ?? "ERROR");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting random images");
                return Result<List<string>>.Failure("Exception occurred while getting random images", "EXCEPTION");
            }
        }
    }
}
