using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WeatherImageGenerator.Domain.Configuration;
using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Weather;

public class WeatherFunction : BaseFunction
{
    private readonly IEnumerable<IWeatherService> _weatherServices;
    private readonly IMemoryCache _cache;
    private readonly WeatherServiceOptions _options;

    public WeatherFunction(
        ILogger<WeatherFunction> logger,
        IEnumerable<IWeatherService> weatherServices,
        IMemoryCache cache,
        IOptions<WeatherServiceOptions> options)
        : base(logger)
    {
        _weatherServices = weatherServices;
        _cache = cache;
        _options = options.Value;
    }

    [Function("GetWeatherStations")]
    public async Task<HttpResponseData> GetWeatherStations(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "weather/stations")] HttpRequestData req)
    {
        var result = await GetWeatherStationsAsync();

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteAsJsonAsync(result);

        return response;
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

    public Result<IWeatherService> GetWeatherService()
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