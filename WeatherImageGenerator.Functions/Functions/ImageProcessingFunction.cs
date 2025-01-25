using Azure.Storage.Queues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Domain.Entities.Image;
using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Services.Weather;

namespace WeatherImageGenerator.Functions.Functions
{
    public class ImageProcessingFunction
    {
        private readonly UnsplashClient _unsplashClient;
        private readonly IImageOverlayService _imageOverlayService;
        private readonly ILogger<ImageProcessingFunction> _logger;
        private readonly IWeatherService _weatherService;
        private readonly QueueClient _queueClient;

        public ImageProcessingFunction(
            UnsplashClient unsplashClient,
            IImageOverlayService imageOverlayService,
            ILogger<ImageProcessingFunction> logger,
            IWeatherService weatherService,
            QueueClient queueClient)
        {
            _unsplashClient = unsplashClient;
            _imageOverlayService = imageOverlayService;
            _logger = logger;
            _weatherService = weatherService;
            _queueClient = queueClient;
        }


        [Function("ProcessWeatherImages")]
        public async Task ProcessWeatherImages([QueueTrigger("weather-image-queue", Connection = "AzureWebJobsStorage")] string queueItem)
        {
            try
            {
                var weatherStations = await GetWeatherDataForStationsAsync();
                if (weatherStations == null || !weatherStations.Any())
                {
                    _logger.LogWarning("No weather stations data available to process.");
                    return;
                }

                var imageUrls = await GetImageUrlsAsync("nature", weatherStations.Count);
                if (imageUrls == null || !imageUrls.Any())
                {
                    _logger.LogWarning("No image URLs retrieved from Unsplash.");
                    return;
                }

                var images = new List<WeatherImage>();
                for (int i = 0; i < weatherStations.Count; i++)
                {
                    var image = await _imageOverlayService.OverlayWeatherDataOnImageAsync(imageUrls[i], weatherStations[i]);
                    if (image != null)
                    {
                        images.Add(image);
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to overlay weather data on image from URL: {imageUrls[i]}");
                    }
                }

                await SaveAndQueueImagesAsync(images, "processed-image-queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process weather images");
            }
        }

        public async Task<List<WeatherStation>> GetWeatherDataForStationsAsync()
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherDataAsync();
                if (weatherData != null && weatherData.Stations != null && weatherData.Stations.Any())
                {
                    return weatherData.Stations.ToList();
                }
                else
                {
                    _logger.LogError("WeatherData is null or contains no stations.");
                    return new List<WeatherStation>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve weather data.");
                return new List<WeatherStation>();
            }
        }

        public async Task<List<string>> GetImageUrlsAsync(string query, int count)
        {
            try
            {
                var result = await _unsplashClient.GetRandomNaturePhotosAsync(query, count);
                if (result.IsSuccess && result.Data != null)
                {
                    return result.Data.Select(img => img.Urls.Regular).ToList();
                }
                else
                {
                    _logger.LogError("Failed to retrieve image URLs from Unsplash: {Error}", result.Error);
                    return new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching image URLs from Unsplash.");
                return new List<string>();
            }
        }

        public async Task SaveAndQueueImagesAsync(List<WeatherImage> images, string queueName)
        {
            foreach (var image in images)
            {
                var blobUrl = await SaveImageToBlobStorageAsync(image);

                // Update the BlobUrl property with the actual Blob URL
                image.BlobUrl = blobUrl;

                // Queue the image for further processing or notification
                var message = JsonSerializer.Serialize(image);
                await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
            }
        }

        public async Task<string> SaveImageToBlobStorageAsync(WeatherImage image)
        {
            // TODO: Implement actual Blob Storage saving logic here.
            // For demonstration, returning a placeholder URL.
            await Task.Delay(100); // Simulate async work
            return $"https://yourstorageaccount.blob.core.windows.net/images/{image.Id}.png";
        }
    }
}
