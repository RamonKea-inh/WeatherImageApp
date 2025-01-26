using System.Text.Json.Serialization;

namespace WeatherImageGenerator.Domain.Models.Response
{
    public class BuienradarResponse
    {
        [JsonPropertyName("buienradar")]
        public Buienradar Buienradar { get; set; } = new();

        [JsonPropertyName("actual")]
        public Actual Actual { get; set; } = new();
    }

    public class Buienradar
    {
        [JsonPropertyName("copyright")]
        public string Copyright { get; set; } = string.Empty;

        [JsonPropertyName("terms")]
        public string Terms { get; set; } = string.Empty;
    }

    public class Actual
    {
        [JsonPropertyName("stationmeasurements")]
        public List<StationMeasurement> Stationmeasurements { get; set; } = new();
    }

    public class StationMeasurement
    {
        [JsonPropertyName("stationid")]
        public int Stationid { get; set; }

        [JsonPropertyName("stationname")]
        public string Stationname { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public decimal Lat { get; set; }

        [JsonPropertyName("lon")]
        public decimal Lon { get; set; }

        [JsonPropertyName("regio")]
        public string Regio { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("weatherdescription")]
        public string Weatherdescription { get; set; } = string.Empty;

        [JsonPropertyName("temperature")]
        public decimal Temperature { get; set; }

        [JsonPropertyName("feeltemperature")]
        public decimal Feeltemperature { get; set; }

        [JsonPropertyName("windspeed")]
        public decimal Windspeed { get; set; }

        [JsonPropertyName("winddirection")]
        public string Winddirection { get; set; } = string.Empty;
    }
}