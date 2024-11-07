using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherImageGenerator.Domain.Entities.Image;
using WeatherImageGenerator.Domain.Models.Response;
using WeatherImageGenerator.Domain.Models.Weather;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IImageService
    {
        Task<Result<List<string>>> GetRandomNatureImagesUrlAsync(string query, int count);
        
    }
}
