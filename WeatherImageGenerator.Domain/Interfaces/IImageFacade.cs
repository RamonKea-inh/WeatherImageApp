using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IImageFacade
    {
        Task<Result<byte[]>> GetRandomNatureImageAsync();
    }
}
