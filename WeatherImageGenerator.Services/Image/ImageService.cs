using Microsoft.Extensions.Logging;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Response;

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

        public async Task<Result<string>> GetRandomImageUrlAsync(string query = "nature")
        {
            try
            {
                var result = await _unsplashClient.GetRandomPhotoAsync(query);
                if (result.IsSuccess && result.Data != null)
                {
                    return Result<string>.Success(result.Data.Urls.Full);
                }
                else
                {
                    _logger.LogError($"Failed to get random image: {result.Error}");
                    return Result<string>.Failure(result.Error ?? "Unknown error", result.ErrorCode ?? "ERROR");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting random image");
                return Result<string>.Failure("Exception occurred while getting random image", "EXCEPTION");
            }
        }
    }
}
