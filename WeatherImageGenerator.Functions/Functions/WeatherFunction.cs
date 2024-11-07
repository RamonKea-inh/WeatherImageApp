using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Weather;

public class WeatherFunction : BaseFunction
{
    private readonly IWeatherFacade _weatherFacade;

    public WeatherFunction(IWeatherFacade weatherFacade,ILogger<WeatherFunction> logger): base(logger)
    {
        _weatherFacade = weatherFacade;
    }

    [Function("GetWeather")]
    public Task<HttpResponseData> GetWeather([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather")] HttpRequestData req)
    {
        return ExecuteAsync(req,
            () => _weatherFacade.GetWeatherInformationAsync(), "GetWeather");
    }

    [Function("GetWeatherStations")]
    public Task<HttpResponseData> GetWeatherStations([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather/stations")] HttpRequestData req)
    {
        return ExecuteAsync(req,
            () => _weatherFacade.GetWeatherStationsAsync(), "GetWeatherStations");
    }

    [Function("GetWeatherStation")]
    public Task<HttpResponseData> GetWeatherStation([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "weather/stations/{stationId}")]HttpRequestData req,string stationId)
    {
        return ExecuteAsync(req,
            () => _weatherFacade.GetStationByIdAsync(stationId), "GetWeatherStation");
    }
}