using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Domain.Enums;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Status;

namespace WeatherImageGenerator.Services.Jobs
{
    public class ImageJobService : IImageJobService
    {
        private readonly ILogger<ImageJobService> _logger;
        private readonly QueueClient _jobQueueClient;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IWeatherService _weatherService;
        private readonly UnsplashClient _unsplashClient;
        private readonly IImageOverlayService _imageOverlayService;

        // Thread-safe dictionary to track job statuses
        private static ConcurrentDictionary<string, ImageJobStatus> _jobStatuses =
            new ConcurrentDictionary<string, ImageJobStatus>();

        public ImageJobService(
            ILogger<ImageJobService> logger,
            QueueClient jobQueueClient,
            IBlobStorageService blobStorageService,
            IWeatherService weatherService,
            UnsplashClient unsplashClient,
            IImageOverlayService imageOverlayService)
        {
            _logger = logger;
            _jobQueueClient = jobQueueClient;
            _blobStorageService = blobStorageService;
            _weatherService = weatherService;
            _unsplashClient = unsplashClient;
            _imageOverlayService = imageOverlayService;
        }

        public async Task<string> StartImageGenerationJobAsync()
        {
            var jobId = Guid.NewGuid().ToString();

            // Create initial job status
            var jobStatus = new ImageJobStatus
            {
                JobId = jobId,
                State = JobState.Pending,
                CreatedAt = DateTime.UtcNow,
                ImageUrls = new List<string>()
            };

            // Store job status
            _jobStatuses[jobId] = jobStatus;

            // Queue job for processing
            await _jobQueueClient.SendMessageAsync(
                Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jobId))
            );

            return jobId;
        }

        public async Task<ImageJobStatus> GetJobStatusAsync(string jobId)
        {
            if (_jobStatuses.TryGetValue(jobId, out var status))
            {
                return status;
            }

            return null;
        }

        // Background job processing method
        public async Task ProcessImageGenerationJobAsync(string jobId)
        {
            try
            {
                // Retrieve weather stations
                var weatherData = await _weatherService.GetWeatherDataAsync();

                // Get random nature images
                var imageResult = await _unsplashClient.GetRandomNaturePhotosAsync(
                    "nature",
                    weatherData.Stations.Count
                );

                if (!imageResult.IsSuccess)
                {
                    throw new Exception("Failed to retrieve images from Unsplash");
                }

                var generatedImages = new List<string>();

                // Generate images for each weather station
                for (int i = 0; i < weatherData.Stations.Count; i++)
                {
                    var station = weatherData.Stations[i];
                    var imageUrl = imageResult.Data[i].Urls.Regular;

                    // Overlay weather data on image
                    var weatherImage = await _imageOverlayService.OverlayWeatherDataOnImageAsync(imageUrl, station);

                    if (weatherImage != null)
                    {
                        try
                        {
                            // Use the new blob storage service
                            var blobName = $"{weatherImage.Id}.png";
                            using var imageStream = new MemoryStream(weatherImage.ImageData);

                            var blobUrl = await _blobStorageService.UploadBlobAsync(
                                "weather-images",
                                blobName,
                                imageStream
                            );

                            generatedImages.Add(blobUrl);
                        }
                        catch (Exception uploadEx)
                        {
                            _logger.LogError(uploadEx, $"Failed to upload image for station {station.StationId}");
                        }
                    }
                }

                // Update job status
                _jobStatuses[jobId] = new ImageJobStatus
                {
                    JobId = jobId,
                    State = generatedImages.Any() ? JobState.Completed : JobState.Failed,
                    CompletedAt = DateTime.UtcNow,
                    ImageUrls = generatedImages,
                    ErrorMessage = !generatedImages.Any() ? "No images could be uploaded" : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing image generation job {jobId}");

                // Update job status to failed
                _jobStatuses[jobId] = new ImageJobStatus
                {
                    JobId = jobId,
                    State = JobState.Failed,
                    CompletedAt = DateTime.UtcNow,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
