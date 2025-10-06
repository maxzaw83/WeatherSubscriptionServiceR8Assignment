
# Weather Subscription Service

This is a lightweight weather subscription service built with Azure Functions, as part of a take-home exercise.

## Features
- Users can subscribe to daily weather updates via a simple API endpoint.
- A daily timer fetches weather data from OpenWeatherMap.
- Notifications are sent to a queue for processing.

## Prerequisites
- .NET 8 SDK
- Docker Desktop
- An Azure Storage Account (Azurite can be used for local development)
- An API key from [OpenWeatherMap](https://openweathermap.org/appid)

## Configuration
To run this project, you need to configure your settings in the `local.settings.json` file.

1.  add a new file  `local.settings.json`.
2.  Update the values:
    ```json
    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "OpenWeatherMapApiKey": "YOUR_OPENWEATHERMAP_API_KEY"
      }
    }
    ```
## How to Run Locally

1.  Clone this repository.
2.  Open the solution in Visual Studio.
3.  Configure your `local.settings.json` file.
4.  Press F5 to start the function app.

## How to Run with Docker

1.  Navigate to the project's root directory in your terminal.
2.  Build the Docker image:
    ```bash
    docker build -t weather-service .
    ```
3.  Run the Docker container, passing in your API key:
    ```bash
    docker run -p 8080:80 -e OpenWeatherMapApiKey="YOUR_API_KEY" weather-service
    ```
