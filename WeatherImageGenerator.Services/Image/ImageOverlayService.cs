using Microsoft.Extensions.Logging;
using SkiaSharp;
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
                // Download image bytes
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);

                // Load image with SkiaSharp
                using (var bitmap = SKBitmap.Decode(imageBytes))
                using (var canvas = new SKCanvas(bitmap))
                {
                    // Create font
                    using var font = new SKFont
                    {
                        Size = 24,
                        Typeface = SKTypeface.FromFamilyName("Arial")
                    };

                    // Create paint for text outline
                    using var blackPaint = new SKPaint
                    {
                        Color = SKColors.Black,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1,
                        IsAntialias = true
                    };

                    // Create paint for text fill
                    using var whitePaint = new SKPaint
                    {
                        Color = SKColors.White,
                        Style = SKPaintStyle.Fill,
                        IsAntialias = true
                    };

                    string text = $"Station: {weatherStation.StationName}\n" +
                                  $"Temperature: {weatherStation.Temperature}°C\n" +
                                  $"Wind: {weatherStation.WindSpeed} km/h {weatherStation.WindDirection}";

                    float y = 30;
                    foreach (var line in text.Split('\n'))
                    {
                        canvas.DrawText(line, 10, y, SKTextAlign.Left, font, blackPaint);
                        canvas.DrawText(line, 10, y, SKTextAlign.Left, font, whitePaint);
                        y += font.Size + 10;
                    }

                    // Convert bitmap back to byte array
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        var imageData = data.ToArray();

                        var weatherImage = new WeatherImage
                        {
                            Id = Guid.NewGuid(),
                            ImageData = imageData,
                            BlobUrl = imageUrl
                        };

                        return weatherImage;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while overlaying weather data on image");
                return null;
            }
        }
    }
}
