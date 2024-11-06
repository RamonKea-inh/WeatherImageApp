using System.Text.Json.Serialization;
using WeatherImageGenerator.Domain.Entities.Weather;

namespace WeatherImageGenerator.Domain.Models.Response
{
    public record BuienradarResponse
    {
        [JsonPropertyName("$id")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("buienradar")]
        public BuienradarInfo Buienradar { get; init; } = new();

        [JsonPropertyName("actual")]
        public ActualInfo Actual { get; init; } = new();

        [JsonPropertyName("stations")]
        public List<WeatherStation> Stations { get; init; } = new();
    }

    public record BuienradarInfo
    {
        [JsonPropertyName("$id")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("copyright")]
        public string Copyright { get; init; } = string.Empty;

        [JsonPropertyName("terms")]
        public string Terms { get; init; } = string.Empty;
    }

    public record ActualInfo
    {
        [JsonPropertyName("$id")]
        public string Id { get; init; } = string.Empty;
    }

    public class UnsplashResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UnsplashUrls Urls { get; set; } = new UnsplashUrls();
    }

    public class UnsplashUrls
    {
        public string Raw { get; set; } = string.Empty;
        public string Regular { get; set; } = string.Empty;
    }
}