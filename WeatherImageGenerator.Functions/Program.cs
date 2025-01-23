using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherImageGenerator.Data.Clients;
using WeatherImageGenerator.Data.Configuration;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Functions.Middleware;
using WeatherImageGenerator.Services.Image;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

        if (env.IsDevelopment())
        {
            config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        }
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddWeatherServices(context.Configuration);

        // Register UnsplashSettings
        services.Configure<UnsplashSettings>(context.Configuration.GetSection("Unsplash"));

        // Register HttpClient and UnsplashClient
        services.AddHttpClient<UnsplashClient>();
        services.AddLogging();

        // Register ImageService
        services.AddScoped<IImageService, ImageService>();
    })
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .Build();

await host.RunAsync();