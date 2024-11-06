using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherImageGenerator.Domain.Configuration;
using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Weather;

namespace WeatherImageGenerator.Services.Weather
{
    public class WeatherFacade : IWeatherFacade
    {
        private readonly IEnumerable<IWeatherService> _weatherServices;
        private readonly ILogger<WeatherFacade> _logger;
        private readonly IMemoryCache _cache;
        private readonly WeatherServiceOptions _options;

        public WeatherFacade(
            IEnumerable<IWeatherService> weatherServices,
            ILogger<WeatherFacade> logger,
            IMemoryCache cache,
            IOptions<WeatherServiceOptions> options)
        {
            _weatherServices = weatherServices ?? throw new ArgumentNullException(nameof(weatherServices));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<Result<WeatherData>> GetWeatherInformationAsync()
        {
            var serviceResult = GetWeatherService();
            if (!serviceResult.IsSuccess)
            {
                return Result<WeatherData>.Failure(
                    serviceResult.Error ?? "Weather service not available",
                    serviceResult.ErrorCode ?? "SERVICE_ERROR");
            }

            try
            {
                var service = serviceResult.Data!;
                WeatherData data;

                if (!_options.EnableCaching)
                {
                    data = await service.GetWeatherDataAsync();
                }
                else
                {
                    var cacheKey = $"WeatherData_{service.SourceName}";
                    data = await _cache.GetOrCreateAsync(cacheKey, async entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromMinutes(_options.CacheDurationMinutes);
                        return await service.GetWeatherDataAsync();
                    }) ?? throw new InvalidOperationException("Cache returned null value");
                }

                _logger.LogInformation("Successfully retrieved weather data from {Source}",
                    service.SourceName);
                return Result<WeatherData>.Success(data);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
            {
                _logger.LogError(ex, "External service error while fetching weather data");
                return Result<WeatherData>.Failure(
                    "Unable to retrieve weather data from external service",
                    "EXTERNAL_SERVICE_ERROR");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching weather data");
                return Result<WeatherData>.Failure(
                    "An unexpected error occurred",
                    "INTERNAL_ERROR");
            }
        }

        public async Task<Result<IReadOnlyList<WeatherStation>>> GetWeatherStationsAsync()
        {
            var weatherResult = await GetWeatherInformationAsync();

            if (!weatherResult.IsSuccess)
            {
                return Result<IReadOnlyList<WeatherStation>>.Failure(
                    weatherResult.Error ?? "Failed to retrieve weather stations",
                    weatherResult.ErrorCode ?? "WEATHER_DATA_ERROR");
            }

            return Result<IReadOnlyList<WeatherStation>>.Success(
                weatherResult.Data?.Stations ?? new List<WeatherStation>());
        }

        public async Task<Result<WeatherStation>> GetStationByIdAsync(string stationId)
        {
            if (!int.TryParse(stationId, out int id))
            {
                return Result<WeatherStation>.Failure(
                    "Invalid station ID format",
                    "INVALID_STATION_ID");
            }

            var stationsResult = await GetWeatherStationsAsync();

            if (!stationsResult.IsSuccess)
            {
                return Result<WeatherStation>.Failure(
                    stationsResult.Error ?? "Failed to retrieve weather stations",
                    stationsResult.ErrorCode ?? "WEATHER_DATA_ERROR");
            }

            var station = stationsResult.Data?.FirstOrDefault(s => s.StationId == id);

            if (station == null)
            {
                return Result<WeatherStation>.Failure(
                    $"Weather station with ID {id} not found",
                    "STATION_NOT_FOUND");
            }

            return Result<WeatherStation>.Success(station);
        }

        private Result<IWeatherService> GetWeatherService()
        {
            var service = _weatherServices.FirstOrDefault();

            if (service == null)
            {
                _logger.LogError("No weather service configured");
                return Result<IWeatherService>.Failure(
                    "Weather service not available",
                    "SERVICE_NOT_CONFIGURED");
            }

            return Result<IWeatherService>.Success(service);
        }
    }
}