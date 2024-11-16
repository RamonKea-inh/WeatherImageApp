using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;
using WeatherImageGenerator.Domain.Entities.Image;
using WeatherImageGenerator.Domain.Entities.Weather;
using WeatherImageGenerator.Domain.Interfaces;

namespace WeatherImageGenerator.Services.Image
{
    public class ImageOverlayService : IImageOverlayService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ImageService> _logger;

        public ImageOverlayService(HttpClient httpClient, ILogger<ImageService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<WeatherImage> OverlayWeatherDataOnImageAsync(string imageUrl, WeatherStation weatherStation)
        {
            try
            {
                var imageStream = await _httpClient.GetStreamAsync(imageUrl);
                var image = await Image.LoadAsync<Rgba32>(imageStream);

                var font = SystemFonts.CreateFont("Arial", 24);
                var text = $"Station: {weatherStation.StationName}\n" +
                           $"Temperature: {weatherStation.Temperature}°C\n" +
                           $"Wind: {weatherStation.WindSpeed} km/h {weatherStation.WindDirection}";

                image.Mutate(ctx => ctx.DrawText(text, font, Color.White, new PointF(10, 10)));

                using var memoryStream = new MemoryStream();
                await image.SaveAsync(memoryStream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                var imageData = memoryStream.ToArray();

                var weatherImage = new WeatherImage
                {
                    Id = Guid.NewGuid(),
                    ImageData = imageData,
                    BlobUrl = imageUrl // This will be updated with the actual Blob URL after uploading
                };

                return weatherImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while overlaying weather data on image");
                return null;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHttpClient<IImageOverlayService, ImageOverlayService>();
            // Register other services
        }
    }
}
