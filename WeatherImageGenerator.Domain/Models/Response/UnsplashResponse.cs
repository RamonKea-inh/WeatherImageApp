namespace WeatherImageGenerator.Domain.Models.Response
{
    public class UnsplashResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UnsplashUrls Urls { get; set; } = new();
    }

    public class UnsplashUrls
    {
        public string Raw { get; set; } = string.Empty;
        public string Full { get; set; } = string.Empty;
        public string Regular { get; set; } = string.Empty;
        public string Small { get; set; } = string.Empty;
        public string Thumb { get; set; } = string.Empty;
    }
}
