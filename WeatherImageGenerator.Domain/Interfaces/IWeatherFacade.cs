using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Models.Weather;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IWeatherFacade
    {
        Task<Result<WeatherData>> GetWeatherInformationAsync();
        Task<Result<IReadOnlyList<WeatherStation>>> GetWeatherStationsAsync();
        Task<Result<WeatherStation>> GetStationByIdAsync(string stationId);
    }
}