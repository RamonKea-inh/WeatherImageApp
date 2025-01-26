using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using WeatherImageGenerator.Domain.Interfaces;
using WeatherImageGenerator.Domain.Models.Status;

namespace WeatherImageGenerator.Functions.Functions
{
    public class ImageJobFunction : BaseFunction
    {
        private readonly IImageJobService _imageJobService;

        public ImageJobFunction(
            ILogger<ImageJobFunction> logger,
            IImageJobService imageJobService)
            : base(logger)
        {
            _imageJobService = imageJobService;
        }

        [Function("StartImageGenerationJob")]
        public async Task<HttpResponseData> StartJob(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "image-jobs")] HttpRequestData req)
        {
            return await ExecuteAsync(req, async () =>
            {
                var jobId = await _imageJobService.StartImageGenerationJobAsync();
                return Result<string>.Success(jobId);
            }, "StartImageGenerationJob");
        }

        [Function("GetImageJobStatus")]
        public async Task<HttpResponseData> GetJobStatus(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "image-jobs/{jobId}")] HttpRequestData req,
            string jobId)
        {
            return await ExecuteAsync(req, async () =>
            {
                var jobStatus = await _imageJobService.GetJobStatusAsync(jobId);

                if (jobStatus == null)
                {
                    return Result<ImageJobStatus>.Failure("Job not found", "JOB_NOT_FOUND");
                }

                return Result<ImageJobStatus>.Success(jobStatus);
            }, "GetImageJobStatus");
        }

        [Function("ProcessImageGenerationJob")]
        public async Task ProcessJob(
            [QueueTrigger("image-generation-queue", Connection = "AzureWebJobsStorage")] string jobId)
        {
            try
            {
                await _imageJobService.ProcessImageGenerationJobAsync(jobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing image generation job {jobId}");
            }
        }
    }
}
