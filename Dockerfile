# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Lafarge-Onboarding.api/Lafarge-Onboarding.api.csproj", "Lafarge-Onboarding.api/"]
COPY ["Lafarge-Onboarding.application/Lafarge-Onboarding.application.csproj", "Lafarge-Onboarding.application/"]
COPY ["Lafarge-Onboarding.domain/Lafarge-Onboarding.domain.csproj", "Lafarge-Onboarding.domain/"]
COPY ["Lafarge-Onboarding.infrastructure/Lafarge-Onboarding.infrastructure.csproj", "Lafarge-Onboarding.infrastructure/"]

RUN dotnet restore "Lafarge-Onboarding.api/Lafarge-Onboarding.api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Lafarge-Onboarding.api"
RUN dotnet build "Lafarge-Onboarding.api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Lafarge-Onboarding.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Lafarge-Onboarding.api.dll"]