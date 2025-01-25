# WeatherImageGenerator


# Workflow

1. Client calls initial HTTP endpoint
2. Server starts async job to generate images
3. Server returns a job/tracking ID
4. Client can poll another endpoint to check job status
5. When job is complete, endpoint returns image URLs from blob storage