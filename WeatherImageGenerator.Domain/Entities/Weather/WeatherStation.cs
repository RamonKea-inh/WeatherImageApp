namespace WeatherImageGenerator.Domain.Entities.Weather
{
    public record WeatherStation(
        int StationId,
        string StationName,
        decimal Latitude,
        decimal Longitude,
        string Region,
        DateTime Timestamp,
        string WeatherDescription,
        decimal Temperature,
        decimal FeelTemperature,
        decimal WindSpeed,
        string WindDirection);
}