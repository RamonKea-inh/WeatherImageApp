using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherImageGenerator.Domain.Exceptions;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Response;
using WeatherImageGenerator.Domain.Models.Weather;

namespace WeatherImageGenerator.Services.Weather
{
    public class BuienradarWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BuienradarWeatherService> _logger;
        private const string BaseUrl = "https://data.buienradar.nl/2.0/feed/json";

        public string SourceName => "Buienradar";

        public BuienradarWeatherService(HttpClient httpClient, ILogger<BuienradarWeatherService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<WeatherData> GetWeatherDataAsync()
        {
            try
            {
                _logger.LogInformation("Fetching weather data from Buienradar");
                var response = await _httpClient.GetStringAsync(BaseUrl);
                var buienradarData = JsonSerializer.Deserialize<BuienradarResponse>(response);

                if (buienradarData == null)
                {
                    throw new WeatherServiceException("Failed to deserialize Buienradar response");
                }

                return new WeatherData
                {
                    Source = SourceName,
                    LastUpdated = DateTime.UtcNow,
                    Stations = buienradarData.Stations.ToList(),
                    Metadata = new Dictionary<string, string>
                    {
                        ["Copyright"] = buienradarData.Buienradar.Copyright,
                        ["Terms"] = buienradarData.Buienradar.Terms
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data from Buienradar");
                throw new WeatherServiceException(
                    "Failed to retrieve weather data from Buienradar",
                    "BUIENRADAR_ERROR",
                    ex);
            }
        }
    }
}