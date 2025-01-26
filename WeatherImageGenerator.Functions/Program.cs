using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherImageGenerator.Data.Clients;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, config) =>
    {
        var env = context.HostingEnvironment;
        config.SetBasePath(Environment.CurrentDirectory)
              .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();

        var builtConfig = config.Build();
        Console.WriteLine($"AzureWebJobsStorage: {builtConfig["AzureWebJobsStorage"]}");
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();

        services.AddWeatherServices(configuration);

        // Register HttpClient and UnsplashClient
        services.AddHttpClient<UnsplashClient>();
        services.AddLogging();
    })
    .Build();

await host.RunAsync();