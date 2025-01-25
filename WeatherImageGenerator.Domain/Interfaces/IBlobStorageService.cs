using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadBlobAsync(string containerName, string blobName, Stream content);
    }
}
