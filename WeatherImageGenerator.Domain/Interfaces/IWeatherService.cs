using WeatherImageGenerator.Domain.Models.Weather;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IWeatherService
    {
        string SourceName { get; }
        Task<WeatherData> GetWeatherDataAsync();
    }
}