# Parameters
param (
    [string]$resourceGroupName,
    [string]$location,
    [string]$functionAppName,
    [string]$storageAccountName,
    [string]$appInsightsName
)

# Publish the .NET project
Write-Host "Publishing the .NET project..."
dotnet publish WeatherImageGenerator.Functions/WeatherImageGenerator.Functions.csproj -c Release -o ./publish

# Create a zip file of the published output
Write-Host "Creating a zip file of the published output..."
Compress-Archive -Path ./publish/* -DestinationPath ./publish.zip

# Create the resource group if it doesn't exist
Write-Host "Creating resource group..."
az group create --name $resourceGroupName --location $location

# Deploy the Bicep template
Write-Host "Deploying the Bicep template..."
az deployment group create --resource-group $resourceGroupName --template-file ./Infrastructure/main.bicep --parameters functionAppName=$functionAppName storageAccountName=$storageAccountName appInsightsName=$appInsightsName location=$location

# Deploy the function app
Write-Host "Deploying the function app..."
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src ./publish.zip

Write-Host "Deployment completed successfully."