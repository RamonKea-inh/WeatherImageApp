namespace WeatherImageGenerator.Domain.Configuration
{
    public class WeatherServiceOptions
    {
        public bool EnableCaching { get; set; }
        public int CacheDurationMinutes { get; set; }
    }
}