using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Domain.Entities.Image;
using WeatherImageGenerator.Domain.Interfaces;

namespace WeatherImageGenerator.Functions.Functions
{
    public class ImageProcessingFunction
    {
        private readonly IWeatherFacade _weatherFacade;
        private readonly UnsplashClient _unsplashClient;
        private readonly IImageOverlayService _imageOverlayService;
        private readonly ILogger<ImageProcessingFunction> _logger;

        public ImageProcessingFunction(IWeatherFacade weatherFacade, UnsplashClient unsplashClient, IImageOverlayService imageOverlayService, ILogger<ImageProcessingFunction> logger)
        {
            _weatherFacade = weatherFacade;
            _unsplashClient = unsplashClient;
            _imageOverlayService = imageOverlayService;
            _logger = logger;
        }

        [Function("ProcessWeatherImages")]
        public async Task ProcessWeatherImages([QueueTrigger("weather-image-queue")] string queueItem)
        {
            try
            {
                var weatherStations = await GetWeatherDataForStationsAsync();
                var imageUrls = await GetImageUrlsAsync("nature", weatherStations.Count);

                var images = new List<WeatherImage>();
                for (int i = 0; i < weatherStations.Count; i++)
                {
                    var image = await _imageOverlayService.OverlayWeatherDataOnImageAsync(imageUrls[i], weatherStations[i]);
                    images.Add(image);
                }

                await SaveAndQueueImagesAsync(images, "processed-image-queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process weather images");
            }
        }

        private async Task SaveAndQueueImagesAsync(List<Image> images, string v)
        {
            throw new NotImplementedException();
        }

        private async Task GetImageUrlsAsync(string v, object count)
        {
            throw new NotImplementedException();
        }

        private async Task GetWeatherDataForStationsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
