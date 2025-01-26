# Weather Image Generator App

Weather Image Generator is a .NET 8 application that generates images with weather data overlays. It uses Azure Functions, Unsplash API, and various other services to fetch weather data and overlay it on images.

## Table of Contents

- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Workflows](#workflows)
- [Contributing](#contributing)
- [License](#license)

## Getting Started

To get started with the Weather Image Generator, follow these steps:

1. Clone the repository:
   
   \`\`\`
   git clone https://github.com/RamonKea-inh/WeatherImageApp.git
   cd weather-image-generator
   \`\`\`

2. Build the solution:
   
   \`\`\`
   dotnet build
   \`\`\`

3. Run the Azure Functions project:
   
   \`\`\`
   cd WeatherImageGenerator.Functions
   func start
   \`\`\`

## Configuration

The application requires configuration settings for various services. These settings are defined in the \`appsettings.json\` and \`local.settings.json\` files.

### appsettings.json

\`\`\`json
{
  "WeatherService": {
    "EnableCaching": true,
    "CacheDurationMinutes": 5
  },
  "Unsplash": {
    "AccessKey": "your-unsplash-access-key"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AzureWebJobsStorage": "your-azure-webjobs-storage-connection-string"
}
\`\`\`

### local.settings.json

\`\`\`json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "WeatherService:EnableCaching": true,
    "WeatherService:CacheDurationMinutes": 5,
    "Unsplash:AccessKey": "your-unsplash-access-key"
  }
}
\`\`\`

## API Endpoints

### Image Job API

- **Start Image Generation Job**
  - **Endpoint**: \`POST /api/image-jobs\`
  - **Description**: Initiates a new image generation job.
  - **Response**: Returns the job ID of the newly created job.

- **Get Image Job Status**
  - **Endpoint**: \`GET /api/image-jobs/{jobId}\`
  - **Description**: Retrieves the status of an image generation job.
  - **Response**: Returns the status of the job.

### Weather API

- **Get Weather Stations**
  - **Endpoint**: \`GET /api/weather/stations\`
  - **Description**: Retrieves weather stations data.
  - **Response**: Returns a list of weather stations.

### Image API

- **Get Images**
  - **Endpoint**: \`GET /api/images\`
  - **Description**: Retrieves a list of images based on a query.
  - **Response**: Returns a list of image URLs.

## Workflows

### Start Image Generation Job

1. **Start Image Generation Job**
   - **Endpoint**: \`POST /api/image-jobs\`
   - **Description**: Initiates a new image generation job.
   - **Response**: Returns the job ID of the newly created job.

2. **Get Image Job Status**
   - **Endpoint**: \`GET /api/image-jobs/{jobId}\`
   - **Description**: Retrieves the status of an image generation job.
   - **Response**: Returns the status of the job.

### Get Weather Stations

1. **Get Weather Stations**
   - **Endpoint**: \`GET /api/weather/stations\`
   - **Description**: Retrieves weather stations data.
   - **Response**: Returns a list of weather stations.

### Get Images

1. **Get Images**
   - **Endpoint**: \`GET /api/images\`
   - **Description**: Retrieves a list of images based on a query.
   - **Response**: Returns a list of image URLs.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
