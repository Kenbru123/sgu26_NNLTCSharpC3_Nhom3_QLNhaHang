
# Build Website ASP.NET Core MVC
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY . .

RUN dotnet restore "Restaurant.Web/Restaurant.Web.csproj"

RUN dotnet publish "Restaurant.Web/Restaurant.Web.csproj" \
    -c Release \
    --no-restore \
    -o /app/publish \
    /p:UseAppHost=false

# Chạy Website
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Restaurant.Web.dll"]
