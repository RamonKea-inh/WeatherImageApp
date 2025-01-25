using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Data.Configuration;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Functions.Middleware;
using WeatherImageGenerator.Services.Image;
using WeatherImageGenerator.Services.Weather;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Register BlobServiceClient
        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["AzureWebJobsStorage"];
            return new BlobServiceClient(connectionString);
        });

        // Register QueueServiceClient for Azure Queue Storage
        services.AddSingleton(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var connectionString = configuration["AzureWebJobsStorage"];
            return new QueueServiceClient(connectionString);
        });

        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();

        // Register Weather Services
        services.AddWeatherServices(context.Configuration);

        // Register UnsplashSettings
        services.Configure<UnsplashSettings>(context.Configuration.GetSection("Unsplash"));

        // Register HttpClient and UnsplashClient
        services.AddHttpClient<UnsplashClient>();
        services.AddLogging();

        // Register ImageService and ImageOverlayService
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IImageOverlayService, ImageOverlayService>();

        // Register WeatherService
        services.AddHttpClient<IWeatherService, WeatherService>();
    })
    .Build();

await host.RunAsync();