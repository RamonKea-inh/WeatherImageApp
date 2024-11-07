using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Exceptions;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Response;
using WeatherImageGenerator.Domain.Models.Weather;

namespace WeatherImageGenerator.Services.Weather
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherService> _logger;
        private const string BaseUrl = "https://data.buienradar.nl/2.0/feed/json";

        public string SourceName => "Buienradar";

        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger)
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

                var stations = buienradarData.Actual.Stationmeasurements.Select(s => new WeatherStation(
                    s.Stationid,
                    s.Stationname,
                    s.Lat,
                    s.Lon,
                    s.Regio,
                    s.Timestamp,
                    s.Weatherdescription,
                    s.Temperature,
                    s.Feeltemperature,
                    s.Windspeed,
                    s.Winddirection
                )).ToList();

                return new WeatherData
                {
                    Source = SourceName,
                    LastUpdated = DateTime.UtcNow,
                    Stations = stations,
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