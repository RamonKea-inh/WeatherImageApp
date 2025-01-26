namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IImageService
    {
        Task<Result<List<string>>> GetRandomNatureImagesUrlAsync(string query = "nature", int count = 1);

    }
}
