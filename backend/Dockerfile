# Multi-stage Dockerfile for ASP.NET Core microservices
# Build: docker build -t ehr-patient:latest -f backend/Dockerfile --build-arg SERVICE=EHRPlatform.Services.Patient .
# Run: docker run -p 5000:5000 -e ConnectionStrings__PatientDb="..." ehr-patient:latest

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG SERVICE=EHRPlatform.Services.Patient
WORKDIR /src

# Copy solution and project files
COPY ["backend/EHRPlatform.sln", "backend/"]
COPY ["backend/src/EHRPlatform.Common/EHRPlatform.Common.csproj", "backend/src/EHRPlatform.Common/"]
COPY ["backend/src/${SERVICE}/${SERVICE}.csproj", "backend/src/${SERVICE}/"]

# Restore dependencies
WORKDIR /src/backend
RUN dotnet restore "EHRPlatform.sln"

# Copy source code
COPY ["backend/src/", "src/"]

# Build
RUN dotnet build "EHRPlatform.sln" -c Release --no-restore

# Publish
FROM build AS publish
ARG SERVICE=EHRPlatform.Services.Patient
RUN dotnet publish "src/${SERVICE}/${SERVICE}.csproj" -c Release -o /app/publish --no-build

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install additional tools for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Set environment
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "EHRPlatform.Services.Patient.dll"]
