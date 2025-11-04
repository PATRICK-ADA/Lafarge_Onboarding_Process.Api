# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies (cached layer)
COPY ["Lafarge-Onboarding.api/Lafarge-Onboarding.api.csproj", "Lafarge-Onboarding.api/"]
COPY ["Lafarge-Onboarding.application/Lafarge-Onboarding.application.csproj", "Lafarge-Onboarding.application/"]
COPY ["Lafarge-Onboarding.domain/Lafarge-Onboarding.domain.csproj", "Lafarge-Onboarding.domain/"]
COPY ["Lafarge-Onboarding.infrastructure/Lafarge-Onboarding.infrastructure.csproj", "Lafarge-Onboarding.infrastructure/"]

RUN dotnet restore "Lafarge-Onboarding.api/Lafarge-Onboarding.api.csproj"

# Copy source code and publish directly (skip separate build step)
COPY ["Lafarge-Onboarding.api/", "Lafarge-Onboarding.api/"]
COPY ["Lafarge-Onboarding.application/", "Lafarge-Onboarding.application/"]
COPY ["Lafarge-Onboarding.domain/", "Lafarge-Onboarding.domain/"]
COPY ["Lafarge-Onboarding.infrastructure/", "Lafarge-Onboarding.infrastructure/"]

WORKDIR "/src/Lafarge-Onboarding.api"
RUN dotnet publish "Lafarge-Onboarding.api.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# Use the official .NET 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 for Cloud Run
EXPOSE 8080

# Cloud Run will set ASPNETCORE_URLS automatically
# For local development, default to port 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Lafarge-Onboarding.api.dll"]