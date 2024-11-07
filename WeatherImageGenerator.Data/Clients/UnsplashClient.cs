using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using WeatherImageGenerator.Domain.Exceptions;
using WeatherImageGenerator.Domain.Models.Response;

namespace WeatherImageGenerator.Data.Clients
{
    public class UnsplashClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _accessKey;
        private readonly ILogger<UnsplashClient> _logger;

        public UnsplashClient(HttpClient httpClient, IConfiguration configuration, ILogger<UnsplashClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _accessKey = configuration["Unsplash:AccessKey"] ?? throw new WeatherServiceException("Unsplash API key not configured", ErrorCode.ImageServiceConfigurationError.ToString());

            _httpClient.BaseAddress = new Uri("https://api.unsplash.com/"); // TODO : Move to configuration
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_accessKey}"); // TODO : Move to configuration
        }

        public async Task<Result<UnsplashResponse>> GetRandomPhotoAsync(string query)
        {
            try
            {
                var response = await _httpClient.GetAsync($"photos/random?query={query}");
                response.EnsureSuccessStatusCode();

                var unsplashResponse = await response.Content
                    .ReadFromJsonAsync<UnsplashResponse>();

                if (unsplashResponse == null)
                {
                    return Result<UnsplashResponse>.Failure(
                        "Failed to deserialize Unsplash response",
                        ErrorCode.ImageServiceDeserializationError.ToString());
                }

                return Result<UnsplashResponse>.Success(unsplashResponse);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch random photo from Unsplash");
                return Result<UnsplashResponse>.Failure(
                    "Failed to fetch image from Unsplash",
                    ErrorCode.ImageServiceApiError.ToString());
            }
        }
    }
}
