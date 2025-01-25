using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherImageGenerator.Domain.Configuration;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Services.Image;
using WeatherImageGenerator.Services.Weather;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register all weather services
        services.AddHttpClient<WeatherService>();
        services.AddSingleton<IWeatherService, WeatherService>();

        // Register all image services
        services.AddSingleton<IImageService, ImageService>();
        services.AddSingleton<IImageOverlayService, ImageOverlayService>();

        services.Configure<WeatherServiceOptions>(
            configuration.GetSection("WeatherService"));

        services.AddMemoryCache();

        return services;
    }
}