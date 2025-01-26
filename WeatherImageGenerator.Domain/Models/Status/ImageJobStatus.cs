using System.Text.Json.Serialization;
using WeatherImageGenerator.Domain.Enums;

namespace WeatherImageGenerator.Domain.Models.Status
{
    public class ImageJobStatus
    {
        public string JobId { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public List<string> ImageUrls { get; set; }
        public string ErrorMessage { get; set; }
    }
}
