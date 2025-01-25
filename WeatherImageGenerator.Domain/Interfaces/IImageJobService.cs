using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherImageGenerator.Domain.Models.Status;

namespace WeatherImageGenerator.Domain.Interfaces
{
    public interface IImageJobService
    {
        Task<string> StartImageGenerationJobAsync();
        Task<ImageJobStatus> GetJobStatusAsync(string jobId);
        Task ProcessImageGenerationJobAsync(string jobId);
    }
}
