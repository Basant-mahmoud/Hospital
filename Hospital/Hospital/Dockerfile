# ===========================
# Base image (Runtime)
# ===========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# ===========================
# Build Stage
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all files
COPY . .

# Restore dependencies for the API project only
RUN dotnet restore "Hospital.csproj"

# Publish the API project
RUN dotnet publish "Hospital.csproj" -c Release -o /app/publish

# ===========================
# Final Stage
# ===========================
FROM base AS final
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Start the API
ENTRYPOINT ["dotnet", "Hospital.dll"]
