using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using WeatherImageGenerator.Data.Configuration;
using WeatherImageGenerator.Domain.Exceptions;
using WeatherImageGenerator.Domain.Models.Response;

namespace WeatherImageGenerator.Data.Clients
{
    public class UnsplashClient
    {
        private readonly HttpClient _httpClient;
        private readonly UnsplashSettings _unsplashSettings;
        private readonly ILogger<UnsplashClient> _logger;

        public UnsplashClient(
            HttpClient httpClient,
            IOptions<UnsplashSettings> unsplashOptions,
            ILogger<UnsplashClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _unsplashSettings = unsplashOptions.Value;

            if (string.IsNullOrEmpty(_unsplashSettings.AccessKey))
            {
                throw new WeatherServiceException(
                    "Unsplash API key not configured",
                    ErrorCode.ImageServiceConfigurationError.ToString());
            }

            _httpClient.BaseAddress = new Uri(_unsplashSettings.BaseAddress);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_unsplashSettings.AccessKey}");
        }

        public async Task<Result<List<UnsplashResponse>>> GetRandomNaturePhotosAsync(string query, int count)
        {
            try
            {
                var response = await _httpClient.GetAsync($"photos/random?query={query}&count={count}");
                response.EnsureSuccessStatusCode();

                var unsplashResponse = await response.Content
                    .ReadFromJsonAsync<List<UnsplashResponse>>();

                if (unsplashResponse == null)
                {
                    return Result<List<UnsplashResponse>>.Failure(
                        "Failed to deserialize Unsplash response",
                        ErrorCode.ImageServiceDeserializationError.ToString());
                }

                return Result<List<UnsplashResponse>>.Success(unsplashResponse);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch random nature photos from Unsplash");
                return Result<List<UnsplashResponse>>.Failure(
                    "Failed to fetch images from Unsplash",
                    ErrorCode.ImageServiceApiError.ToString());
            }
        }
    }
}
