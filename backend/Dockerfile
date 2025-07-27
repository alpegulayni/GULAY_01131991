## Build and run the VideoApi using multi‑stage builds

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY VideoApi/VideoApi.csproj ./VideoApi/
RUN dotnet restore ./VideoApi/VideoApi.csproj

# Copy the remaining source code
COPY VideoApi/ ./VideoApi/

# Build the application
WORKDIR /src/VideoApi
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VideoApi.dll"]