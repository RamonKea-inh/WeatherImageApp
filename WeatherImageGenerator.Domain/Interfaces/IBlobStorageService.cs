namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadBlobAsync(string containerName, string blobName, Stream content);
    }
}
