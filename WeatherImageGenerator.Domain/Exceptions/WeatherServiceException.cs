namespace WeatherImageGenerator.Domain.Exceptions
{
    public class WeatherServiceException : Exception
    {
        public string ErrorCode { get; }

        public WeatherServiceException(string message, string errorCode = "WEATHER_SERVICE_ERROR", Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}