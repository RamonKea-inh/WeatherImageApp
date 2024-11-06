using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherImageGenerator.Domain.Configuration;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Services.Weather;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register all weather services
        services.AddHttpClient<BuienradarWeatherService>();
        services.AddSingleton<IWeatherService, BuienradarWeatherService>();

        // Could add more weather services here
        // services.AddSingleton<IWeatherService, OpenWeatherMapService>();

        services.Configure<WeatherServiceOptions>(
            configuration.GetSection("WeatherService"));

        services.AddMemoryCache();
        services.AddSingleton<IWeatherFacade, WeatherFacade>();

        return services;
    }
}