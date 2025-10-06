# Base image for the final stage
FROM mcr.microsoft.com/azure-functions/dotnet:4-dotnet8.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 8080

# Build stage to compile and publish the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj file first to leverage Docker layer caching
# This layer only gets rebuilt if  project dependencies change
COPY WeatherSubscriptionService.csproj ./
RUN dotnet restore ./WeatherSubscriptionService.csproj

# Copy the rest of the source code
COPY . ./

# Build and publish the application
RUN dotnet publish ./WeatherSubscriptionService.csproj -c Release -o /app/publish

# Final stage with the published application
FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=build /app/publish .

# Set environment variables for the Functions runtime
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true