using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherImageGenerator.Data.Configuration;
using WeatherImageGenerator.Domain.Configuration;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Services.Image;
using WeatherImageGenerator.Services.Jobs;
using WeatherImageGenerator.Services.Storage;
using WeatherImageGenerator.Services.Weather;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeatherServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IBlobStorageService>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration["AzureWebJobsStorage"];
            return new BlobStorageService(connectionString);
        });

        services.AddSingleton(sp =>
        {
            var connectionString = configuration["AzureWebJobsStorage"];
            return new QueueServiceClient(connectionString);
        });

        // Configure Azure Queue and Blob clients
        services.AddSingleton(sp =>
        {
            var queueServiceClient = sp.GetRequiredService<QueueServiceClient>();
            var queueClient = queueServiceClient.GetQueueClient("image-generation-queue");

            queueClient.CreateIfNotExists();

            return queueClient;
        });


        // Register all weather services
        services.AddHttpClient<WeatherService>();
        services.AddSingleton<IWeatherService, WeatherService>();

        // Register all image services
        services.AddSingleton<IImageService, ImageService>();
        services.AddSingleton<IImageOverlayService, ImageOverlayService>();
        services.AddSingleton<IImageJobService, ImageJobService>();

        services.Configure<UnsplashSettings>(configuration.GetSection("Unsplash"));

        services.Configure<WeatherServiceOptions>(configuration.GetSection("WeatherService"));

        services.AddMemoryCache();

        return services;
    }
}