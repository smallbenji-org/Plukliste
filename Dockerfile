# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage for ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PluckFish/PluckFish.csproj", "./"]
RUN dotnet restore "./PluckFish.csproj"
COPY ./PluckFish . 
WORKDIR "/src"
RUN dotnet build "./PluckFish.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish ASP.NET Core app
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PluckFish.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PluckFish.dll"]