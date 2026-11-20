
# Build ứng dụng bằng .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY . .

RUN dotnet restore "Restaurant.API/Restaurant.API.csproj"

RUN dotnet publish "Restaurant.API/Restaurant.API.csproj" \
    -c Release \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# Image chạy Web API
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Restaurant.API.dll"]
