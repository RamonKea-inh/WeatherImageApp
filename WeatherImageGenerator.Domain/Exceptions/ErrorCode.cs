using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherImageGenerator.Domain.Exceptions
{
    public enum ErrorCode
    {
        // Weather Service Errors (1000-1099)
        WeatherServiceUnavailable = 1000,
        InvalidWeatherData = 1001,
        WeatherDataNotFound = 1002,
        LocationNotSupported = 1003,

        // Image Service Errors (2000-2099)
        ImageServiceUnavailable = 2000,
        ImageNotFound = 2001,
        ImageParsingError = 2002,
        ImageServiceConfigurationError = 2003,
        ImageServiceApiError = 2004,
        ImageServiceDownloadError = 2005,
        ImageServiceDeserializationError = 2006,

        // Image Processing Errors (3000-3099)
        ImageProcessingError = 3000,
        FontNotAvailable = 3001,
        ImageGenerationFailed = 3002,

        // General Errors (9000-9099)
        UnexpectedError = 9000,
        InvalidRequest = 9001,
        ServiceTimeout = 9002,
        InvalidImageFormat = 9003
    }
}
