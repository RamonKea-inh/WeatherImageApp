using WeatherImageGenerator.Domain.Entities.Weather;

namespace WeatherImageGenerator.Domain.Models.Weather
{
    public record WeatherData
    {
        public string Source { get; init; } = string.Empty;
        public DateTime LastUpdated { get; init; }
        public IReadOnlyList<WeatherStation> Stations { get; init; } = new List<WeatherStation>();
        public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
    }
}