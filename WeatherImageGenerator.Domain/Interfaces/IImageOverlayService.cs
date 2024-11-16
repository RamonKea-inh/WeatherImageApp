using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherImageGenerator.Domain.Entities.Image;
using WeatherImageGenerator.Domain.Entities.Weather;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IImageOverlayService
    {
        Task<WeatherImage> OverlayWeatherDataOnImageAsync(string imageUrl, WeatherStation weatherStation);
    }
}
