using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IImageService
    {
        Task<Result<string>> GetRandomImageUrlAsync(string query = "nature");
        Task<Result<byte[]>> DownloadImageAsync(string imageUrl);
    }
}
